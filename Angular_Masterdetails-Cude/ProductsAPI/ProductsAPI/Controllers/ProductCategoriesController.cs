using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsAPI.Data;
using ProductsAPI.Models;

namespace ProductsAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ProductCategoriesController : ControllerBase
    {
        private readonly ProductsAPIContext _context;

        public ProductCategoriesController(ProductsAPIContext context)
        {
            _context = context;
        }

        // GET: ProductCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetProductCategory()
        {
            return await _context.ProductCategory.Include(pc => pc.Products).ToListAsync();
        }

        // GET: ProductCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductCategory>> GetProductCategory(int id)
        {
            var productCategory = await _context.ProductCategory.Include(pc => pc.Products).SingleAsync(pc => pc.ProductCategoryID == id);

            if (productCategory == null)
            {
                return NotFound();
            }

            return productCategory;
        }

        // PUT: ProductCategories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductCategory(int id, ProductCategory productCategory)
        {
           
            if (id != productCategory.ProductCategoryID)
            {
                return BadRequest();
            }
            var existing = await _context.ProductCategory.FirstOrDefaultAsync(x => x.ProductCategoryID == id);
            if (existing == null) { return NotFound(); }
            existing.Name = productCategory.Name;
            _context.Database.ExecuteSqlInterpolated($"DELETE FROM Product WHERE ProductCategoryID={id}");
            if(productCategory.Products !=null)
            {
                foreach (var p in productCategory.Products)
                {
                    _context.Product.Add(new Product { ProductCategoryID = id, Name = p.Name, ProductNumber = p.ProductNumber, Color = p.Color, StandardCost = p.StandardCost, ListPrice = p.ListPrice, Size = p.Size, Weight = p.Weight });
                }
            }
            

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductCategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: ProductCategories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProductCategory>> PostProductCategory(ProductCategory productCategory)
        {
            _context.ProductCategory.Add(productCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductCategory", new { id = productCategory.ProductCategoryID }, productCategory);
        }

        // DELETE: ProductCategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductCategory(int id)
        {
            var productCategory = await _context.ProductCategory.FindAsync(id);
            if (productCategory == null)
            {
                return NotFound();
            }

            _context.ProductCategory.Remove(productCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductCategoryExists(int id)
        {
            return _context.ProductCategory.Any(e => e.ProductCategoryID == id);
        }
    }
}
