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
        public async Task<ActionResult> AddProduct(Product product)
        {
            if (product == null)
            {
                return BadRequest("Отсутствуют данные о продуктах для добавления");
            }

            _context.Product.Add(product);

            await _context.SaveChangesAsync();

            return Ok("Продукты успешно добавлены");
        }

        
            // GET: api/product/smartphones
            [HttpGet("smartphones")]
            public async Task<ActionResult<IEnumerable<Product>>> GetSmartphones()
            {
                // Получаем все продукты субкатегории "Smartphones"
                var products = await _context.Product
                    .Where(p => p.SubCategoryId == 1)
                    .ToListAsync();

                if (products == null || products.Count == 0)
                {
                    return NotFound("Продукты не найдены");
                }

                return Ok(products);
            }
        [HttpGet("GetSubCategory/{subCategoryId}")]
        public async Task<ActionResult<SubCategory>> GetSubCategory(int subCategoryId)
        {
            var subCategory = await _context.SubCategory.FindAsync(subCategoryId);

            if (subCategory == null)
            {
                return NotFound(); // Или другой код статуса в зависимости от ваших требований
            }

            return Ok(subCategory);
        }

        [HttpGet("Smart")]
        public ActionResult<IEnumerable<Product>> GetSmartphonesProducts()
        {
            var smartphonesProducts = _context.Product
                .Include(p => p.SubCategory)
                .Where(p => p.SubCategory.Name == "Smartphones")
                .ToList();

            return Ok(smartphonesProducts);
        }

        
    }
}
