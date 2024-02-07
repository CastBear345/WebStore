using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebStore.Model;

namespace WebStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {
        private readonly ApplicationContext _context;
        public SubCategoryController(ApplicationContext context)
        {
            _context = context;

        }

        // To retrieve all products from subcategories
        [HttpGet("{SubCategoryName}")]
        [Authorize]
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
