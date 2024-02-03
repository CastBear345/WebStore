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
        
        private readonly ApplicationContext _context; // Замените WebStoreDbContext на ваш контекст базы данных

        public ProductController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _context.Products
                .Select(p => p.Name)
                .ToList();

            return Ok(products);
        }
    }
}
