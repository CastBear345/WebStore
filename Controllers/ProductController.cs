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
            var products = _context.Product
                .Select(p => p.Name)
                .ToList();

            return Ok(products);

        }
        [HttpGet("get/{productId}")]
        public async Task<ActionResult<Product>> GetProduct(int productId)
        {

            // Находим продукт в базе данных по идентификатору
            var product = await _context.Product.FindAsync(productId);

            // Если продукт не найден, возвращаем NotFound
            if (product == null)
            {
                return NotFound();
            }

            // Возвращаем продукт в формате JSON
            return Ok(product);
        }

        [HttpPost("insert")]
        public async Task<ActionResult> InsertProduct()
        {
            // Создаем новый объект Product
            var newProduct = new Product
            {
                Name = "iPhone 12",
                ImageURL = "iphone12.jpg",
                SubCategoryId = 1,
                ListOfProductsId = 1,
                Description = "Latest iPhone model",
                Price = 999.99m,
                CountOfLikes = 15
            };

            // Добавляем новый продукт в контекст
            _context.Product.Add(newProduct);

            // Сохраняем изменения в базе данных
            await _context.SaveChangesAsync();

            return Ok("Продукт успешно добавлен");
        }
        [HttpPost]
        public async Task<ActionResult> AddProducts(Product products)
        {
            if (products == null)
            {
                return BadRequest("Отсутствуют данные о продуктах для добавления");
            }
            var newProduct = products;
            _context.Product.AddRange(newProduct);

            await _context.SaveChangesAsync();

            return Ok("Продукты успешно добавлены");
        }

    }
}
