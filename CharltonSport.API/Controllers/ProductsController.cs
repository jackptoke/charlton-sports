using CharltonSport.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CharltonSport.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _shopContext;

        public ProductsController(ShopContext context)
        {
            _shopContext = context;
            _shopContext.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            return Ok(await _shopContext.Products.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(long id)
        {
            var product = await _shopContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> AddProduct(Product newProduct)
        {
            
            _shopContext.Products.Add(newProduct);
            await _shopContext.SaveChangesAsync();
            return CreatedAtAction("GetProduct", new { id = newProduct.Id }, newProduct);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveProduct(long id)
        {
            var product = await _shopContext.Products.FindAsync(id);
            if (product == null)
            {
                return BadRequest();
            }
            _shopContext.Products.Remove(product);
            await _shopContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost]
        [Route("delete")]
        public async Task<ActionResult<List<Product>>> RemoveProducts([FromQuery] long[] ids)
        {
            var products = new List<Product>();
            foreach(var id in ids)
            {
                var product = await _shopContext.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                products.Add(product);
            }

            _shopContext.RemoveRange(products);
            await _shopContext.SaveChangesAsync();
            return Ok(products);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> UpdateProduct(long id, [FromBody] Product newProduct)
        {
            
            if (id != newProduct.Id)
            {
                return BadRequest();
            }
            _shopContext.Entry(newProduct).State = EntityState.Modified;
            try
            {
                await _shopContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException exception)
            {
                if (!_shopContext.Products.Any(p => p.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw exception;
                }

            }
            return NoContent();
        }
    }
}
