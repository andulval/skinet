using System;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")] //how server know where to sent incoming https request - [controller] is replaced by class name minus word Controller of this class
public class ProductsController : ControllerBase
{
    private readonly StoreContext _context; // Declare the context field

    // Constructor to inject the StoreContext
    public ProductsController(StoreContext context)
    {
        _context = context; // Assign the injected context to the field
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        return await _context.Products.ToListAsync(); // Use the context field
    }

    [HttpGet("{id:int}")] //api/products/2
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null) return NotFound();

        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        //konwencjonalnie Entity framework patrzy w requestcie do body, aby sprawdzic czy wystepuje obiekt 
        //* public async Task<ActionResult<Product>> CreateProduct([FromBody]Product product) - jesli nie uzywalibysmy na poczatku pliku '[ApiController]' to przy parametrze trzeba by dodać '[FromBody]'. 

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product;
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (product.Id != id || !ProductExists(id))
        {
            return BadRequest("Cannot update this product");
        }
        //now tell Enitity framework that this product (as argument) is a Product Entitity and the Entity framework should track its state.
        _context.Entry(product).State = EntityState.Modified; //we tell Entity Framework's Tracker that product has benn modified so when we call saveChange will work properly

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        // if(ProductExists(id)){
        //      return BadRequest("Cannot remove this product");
        // }
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();
        _context.Products.Remove(product); //now Entity framework track this product removal
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(x => x.Id == id);
    }

}
