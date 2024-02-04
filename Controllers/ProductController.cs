using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebStore.Model;

namespace WebStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly ApplicationContext _context;
        public ProductController(ApplicationContext context)
        {
            _context = context;

        }


        // Output of all products
        [HttpGet("getProducts")]
        public IActionResult GetProducts()
        {
            // Find all products
            var products = _context.Product
                .Select(p => p.Name)
                .ToList();

            // Return all products
            return Ok(products);
        }


        // Product output by Id
        [HttpGet("getProduct/{productId}")]
        public async Task<ActionResult<Product>> GetProduct(int productId)
        {
            // Finding a product in the database by identifier
            var product = await _context.Product.FindAsync(productId);

            // If the product is not found, return NotFound
            if (product == null)
            {
                return NotFound();
            }

            // Returning the product in JSON format
            return Ok(product);
        }


        // Adding a product
        [HttpPost("addProduct")]
        public async Task<ActionResult> AddProduct(Product product)
        {
            // Check product for null
            if (product == null)
            {
                return BadRequest("There are no product data to add");
            }

            // Save data
            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            return Ok("Products added successfully");
        }


        // To retrieve all products from subcategories
        [HttpGet("{SubCategoryName}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsFromSubCategory(string SubCategoryName)
        {
            // Receiving all products of a subcategory.
            var products = await _context.Product
                .Include(p => p.SubCategory)
                .Where(p => p.SubCategory.Name == SubCategoryName)
                .ToListAsync();

            if (products == null || products.Count == 0)
            {
                return NotFound("Продукты не найдены");
            }

            return Ok(products);
        }

    }
}
