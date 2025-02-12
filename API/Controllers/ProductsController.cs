using System;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")] //how server know where to sent incoming https request - [controller] is replaced by class name minus word Controller of this class
public class ProductsController(IGenericRepository<Product> repo) : ControllerBase
{
    //!old way of assign context:
    // private readonly StoreContext _context; // Declare the context field

    // // Constructor to inject the StoreContext
    // public ProductsController(StoreContext context)
    // {
    //     _context = context; // Assign the injected context to the field
    // }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type, string? sort)
    {
        var spec = new ProductFilterSortPaginationSpecification(brand, type, sort);
        var products = await repo.ListAsync(spec);
        return Ok(products); // Use the context field
    }

    [HttpGet("{id:int}")] //api/products/2
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await repo.GetByIdAsync(id);

        if (product == null) return NotFound();

        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        //konwencjonalnie Entity framework patrzy w requestcie do body, aby sprawdzic czy wystepuje obiekt 
        //* public async Task<ActionResult<Product>> CreateProduct([FromBody]Product product) - jesli nie uzywalibysmy na poczatku pliku '[ApiController]' to przy parametrze trzeba by dodać '[FromBody]'. 

        repo.Add(product);

        if (await repo.SaveAllAsync())
        {
            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        return BadRequest("Problem creating a product");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (product.Id != id || !ProductExists(id))
        {
            return BadRequest("Cannot update this product");
        }
        //now tell Enitity framework that this product (as argument) is a Product Entitity and the Entity framework should track its state.
        // _context.Entry(product).State = EntityState.Modified; //we tell Entity Framework's Tracker that product has benn modified so when we call saveChange will work properly
        repo.Update(product);
        if (await repo.SaveAllAsync())
        {
            return NoContent();
        }

        return BadRequest("Problem updating the product");

    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        // if(ProductExists(id)){
        //      return BadRequest("Cannot remove this product");
        // }
        // var product = await _context.Products.FindAsync(id);
        // if (product == null) return NotFound();
        // _context.Products.Remove(product); //now Entity framework track this product removal
        // await _context.SaveChangesAsync();

        var product = await repo.GetByIdAsync(id);
        if (product == null) return NotFound();
        repo.Remove(product); //now Entity framework track this product removal
        if (await repo.SaveAllAsync())
        {
            return NoContent();
        }

        return BadRequest("Problem deleting the product");
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        var spec = new BrandListSpecification();

        return Ok(await repo.ListAsync(spec));
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        var spec = new TypeListSpecification();

        return Ok(await repo.ListAsync(spec));
    }

    private bool ProductExists(int id)
    {
        return repo.Exists(id);
    }

}


// using System;
// using Core.Entities;
// using Core.Interfaces;
// using Infrastructure.Data;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;

// namespace API.Controllers;

// [ApiController]
// [Route("api/[controller]")] //how server know where to sent incoming https request - [controller] is replaced by class name minus word Controller of this class
// public class ProductsController(IProductRepository repo) : ControllerBase
// {
//     //!old way of assign context:
//     // private readonly StoreContext _context; // Declare the context field

//     // // Constructor to inject the StoreContext
//     // public ProductsController(StoreContext context)
//     // {
//     //     _context = context; // Assign the injected context to the field
//     // }

//     [HttpGet]
//     public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type, string? sort)
//     {
//         return Ok(await repo.GetProductsAsync(brand, type, sort)); // Use the context field
//     }

//     [HttpGet("{id:int}")] //api/products/2
//     public async Task<ActionResult<Product>> GetProduct(int id)
//     {
//         var product = await repo.GetProductByIdAsync(id);

//         if (product == null) return NotFound();

//         return product;
//     }

//     [HttpPost]
//     public async Task<ActionResult<Product>> CreateProduct(Product product)
//     {
//         //konwencjonalnie Entity framework patrzy w requestcie do body, aby sprawdzic czy wystepuje obiekt 
//         //* public async Task<ActionResult<Product>> CreateProduct([FromBody]Product product) - jesli nie uzywalibysmy na poczatku pliku '[ApiController]' to przy parametrze trzeba by dodać '[FromBody]'. 

//         repo.AddProduct(product);

//         if (await repo.SaveChangesAsync())
//         {
//             return CreatedAtAction("GetProduct", new { id = product.Id }, product);
//         }

//         return BadRequest("Problem creating a product");
//     }

//     [HttpPut("{id:int}")]
//     public async Task<ActionResult> UpdateProduct(int id, Product product)
//     {
//         if (product.Id != id || !ProductExists(id))
//         {
//             return BadRequest("Cannot update this product");
//         }
//         //now tell Enitity framework that this product (as argument) is a Product Entitity and the Entity framework should track its state.
//         // _context.Entry(product).State = EntityState.Modified; //we tell Entity Framework's Tracker that product has benn modified so when we call saveChange will work properly
//         repo.UpdateProduct(product);
//         if (await repo.SaveChangesAsync())
//         {
//             return NoContent();
//         }

//         return BadRequest("Problem updating the product");

//     }

//     [HttpDelete("{id:int}")]
//     public async Task<ActionResult> DeleteProduct(int id)
//     {
//         // if(ProductExists(id)){
//         //      return BadRequest("Cannot remove this product");
//         // }
//         // var product = await _context.Products.FindAsync(id);
//         // if (product == null) return NotFound();
//         // _context.Products.Remove(product); //now Entity framework track this product removal
//         // await _context.SaveChangesAsync();

//         var product = await repo.GetProductByIdAsync(id);
//         if (product == null) return NotFound();
//         repo.DeleteProduct(product); //now Entity framework track this product removal
//         if (await repo.SaveChangesAsync())
//         {
//             return NoContent();
//         }

//         return BadRequest("Problem deleting the product");
//     }

//     [HttpGet("brands")]
//     public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
//     {
//         return Ok(await repo.GetBrandsAsync());
//     }

//     [HttpGet("types")]
//     public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
//     {
//         return Ok(await repo.GetTypesAsync());
//     }

//     private bool ProductExists(int id)
//     {
//         return repo.ProductExists(id);
//     }

// }
