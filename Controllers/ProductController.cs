using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebStore.Model;
using WebStore.ModelDTO;

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

        [HttpGet("GetSubcategories")]
        public IActionResult GetSubcategories()
        {
            // Find all products
            var products = _context.MainCategory
                //.Select(p => p.Name)
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


        //To delete a product
        [HttpDelete("deleteProduct/{productId}")]
        public async Task<ActionResult> DeleteProduct(int productId)
        {
            var product = _context.Product
                .Include(p => p.Reviews)    
                .Include(p => p.ShoppingCartProducts)
                .FirstOrDefault(p => p.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            // Отсоединить связанные сущности
            _context.Entry(product).State = EntityState.Detached;

            // Удалить отношения
            foreach (var review in product.Reviews)
            {
                _context.Entry(review).State = EntityState.Detached;
            }

            foreach (var shoppingCartProduct in product.ShoppingCartProducts)
            {
                _context.Entry(shoppingCartProduct).State = EntityState.Detached;
            }
            //foreach (var review in product.Reviews)
            //{
            //    _context.Reviews.Remove(review);  // Удаляем каждый отзыв явно
            //}

            //foreach (var shoppingCartProduct in product.ShoppingCartProducts)
            //{
            //    _context.ShoppingCartProducts.Remove(shoppingCartProduct);  // Удаляем каждый элемент корзины явно
            //}

            // Теперь удалить продукт
            _context.Product.Remove(product);
            _context.SaveChanges();

            return Ok($"The {product.Name} has been deleted");
        }


        //To edit a product
        [HttpPut("EditProduct2/{productId}")]
        public async Task<IActionResult> EditProduct(int productId, [FromBody] ProductDTO updatedProductDTO)
        {
            try
            {
                var existingProduct = await _context.Product.FindAsync(productId);

                if (existingProduct == null)
                {
                    return NotFound(); // Если продукт с заданным идентификатором не найден
                }

                // Обновление свойств продукта
                existingProduct.Name = updatedProductDTO.Name;
                existingProduct.ImageURL = updatedProductDTO.ImageURL;
                existingProduct.SubCategoryId = updatedProductDTO.SubCategoryId;
                existingProduct.ListOfProductsId = updatedProductDTO.ListOfProductsId;
                existingProduct.Description = updatedProductDTO.Description;
                existingProduct.Price = updatedProductDTO.Price;
                existingProduct.CountOfLikes = updatedProductDTO.CountOfLikes;


                // Сохранение изменений
                await _context.SaveChangesAsync();

                return Ok(); // Успешное редактирование
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }






        //редактирование и удаление товаров.
    }
}
