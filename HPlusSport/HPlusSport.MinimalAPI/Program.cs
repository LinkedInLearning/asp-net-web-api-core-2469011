using HPlusSport.API.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ShopContext>(options =>
{
    options.UseInMemoryDatabase("Shop");
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet("/products", async (ShopContext _context) =>
{
    await _context.Database.EnsureCreatedAsync();
    return Results.Ok(await _context.Products.ToListAsync());
});

app.MapGet("/products/{id}", async (int id, ShopContext _context) =>
{
    var product = await _context.Products.FindAsync(id);

    if (product == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(product);
}).WithName("GetProduct");

app.MapPost("/products", async (Product product, ShopContext _context) =>
{
    _context.Products.Add(product);
    await _context.SaveChangesAsync();

    return Results.CreatedAtRoute("GetProduct", new { id = product.Id }, product);
});

app.MapPut("/products/{id}", async (int id, Product product, ShopContext _context) =>
{
    if (id != product.Id)
    {
        return Results.BadRequest();
    }

    _context.Entry(product).State = EntityState.Modified;

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!_context.Products.Any(p => p.Id == id))
        {
            return Results.NotFound();
        }
        else
        {
            throw;
        }
    }

    return Results.NoContent();
});

app.MapDelete("/products/{id}", async (int id, ShopContext _context) =>
{
    var product = await _context.Products.FindAsync(id);
    if (product == null)
    {
        return Results.NotFound();
    }

    _context.Products.Remove(product);
    await _context.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();