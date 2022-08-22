using CharltonSport.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CharltonSport.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/products")]
    // [Route("products")]   <= if API version is specified in the header
    [ApiController]
    public class ProductsV1Controller : ControllerBase
    {
        private readonly ShopContext _shopContext;

        public ProductsV1Controller(ShopContext context)
        {
            _shopContext = context;
            _shopContext.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllProducts([FromQuery] ProductQueryParameters queryParameters)
        {
            IQueryable<Product> products = _shopContext.Products;
            
            // Filtering only products that are more expensive than the min price
            if (queryParameters.MinPrice != null)
            {
                products = products.Where(p => p.Price >= queryParameters.MinPrice);
            }

            // Filtering only products that are cheaper than the max price
            if (queryParameters.MaxPrice != null)
            {
                products = products.Where(p => p.Price <= queryParameters.MaxPrice);
            }
            
            // Searching for substring within the SKU
            if (!string.IsNullOrEmpty(queryParameters.Sku))
            {
                products = products.Where(p => p.Sku.ToLower().Contains(queryParameters.Sku.ToLower()));
            }
            
            // Searching for substring within the name
            if (!string.IsNullOrEmpty(queryParameters.Name))
            {
                string name = queryParameters.Name.ToLower();
                products = products.Where(p => p.Name.ToLower().Contains(name) 
                || p.Sku.ToLower().Contains(name) || p.Description.ToLower().Contains(name));
            }

            // Sort the product by the requested field
            if (!string.IsNullOrEmpty(queryParameters.SortBy))
            {   // , BindingFlags.IgnoreCase, null, null, Array.Empty<Type>(), null
                if (typeof(Product).GetProperty(queryParameters.SortBy) != null)
                {
                    products = products.OrderByCustom(queryParameters.SortBy, queryParameters.SortOrder);
                }
            }

            // Obtaining products on the specified page in the quantity that is requested
            products = products.Skip((queryParameters.Page - 1) * queryParameters.Size)
                .Take(queryParameters.Size);
            return Ok(await products.ToListAsync());
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

    [ApiVersion("2.0")]
    [Route("api/v{v:apiVersion}/products")]
    // [Route("products")]  <= if API version is specified in the header
    [ApiController]
    public class ProductsV2Controller : ControllerBase
    {
        private readonly ShopContext _shopContext;

        public ProductsV2Controller(ShopContext context)
        {
            _shopContext = context;
            _shopContext.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllProducts([FromQuery] ProductQueryParameters queryParameters)
        {
            IQueryable<Product> products = _shopContext.Products.Where(p => p.IsAvailable == true);

            // Filtering only products that are more expensive than the min price
            if (queryParameters.MinPrice != null)
            {
                products = products.Where(p => p.Price >= queryParameters.MinPrice);
            }

            // Filtering only products that are cheaper than the max price
            if (queryParameters.MaxPrice != null)
            {
                products = products.Where(p => p.Price <= queryParameters.MaxPrice);
            }

            // Searching for substring within the SKU
            if (!string.IsNullOrEmpty(queryParameters.Sku))
            {
                products = products.Where(p => p.Sku.ToLower().Contains(queryParameters.Sku.ToLower()));
            }

            // Searching for substring within the name
            if (!string.IsNullOrEmpty(queryParameters.Name))
            {
                string name = queryParameters.Name.ToLower();
                products = products.Where(p => p.Name.ToLower().Contains(name)
                || p.Sku.ToLower().Contains(name) || p.Description.ToLower().Contains(name));
            }

            // Sort the product by the requested field
            if (!string.IsNullOrEmpty(queryParameters.SortBy))
            {   // , BindingFlags.IgnoreCase, null, null, Array.Empty<Type>(), null
                if (typeof(Product).GetProperty(queryParameters.SortBy) != null)
                {
                    products = products.OrderByCustom(queryParameters.SortBy, queryParameters.SortOrder);
                }
            }

            // Obtaining products on the specified page in the quantity that is requested
            products = products.Skip((queryParameters.Page - 1) * queryParameters.Size)
                .Take(queryParameters.Size);
            return Ok(await products.ToListAsync());
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
            foreach (var id in ids)
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
