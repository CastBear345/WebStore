using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using WebStore.Model.ModelDTO;
using WebStore.Model;

namespace WebStore.Controllers
{
    [Route("products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationContext _context;
        public ProductController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("{subcategoryId}")]
        public IActionResult GetProductsBySubCategory(int subcategoryId)
        {
            if (_context.SubCategory.FirstOrDefault(s => s.Id == subcategoryId) == null)
            {
                return NotFound();
            }

            var products = _context.Product.Where(c => c.SubCategoryId == subcategoryId).ToList();

            return Ok(products);
        }
        
        [HttpGet("product/{productId}", Name = "GetProductById")]
        public async Task<ActionResult<Product>> GetProduct(int productId)
        {
            // Finding a product in the database by identifier
            var product = await _context.Product.FindAsync(productId);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult> AddProduct(ProductDTO productDTO)
        { 
            if (productDTO == null)
            {
                return BadRequest("There are no product data to add");
            }

            var newProduct = new Product();

            newProduct.Name = productDTO.Name;
            newProduct.ImageURL = productDTO.ImageURL;
            newProduct.SubCategoryId = productDTO.SubCategoryId;
            newProduct.ListOfProductsId = 1;
            newProduct.Description = productDTO.Description;
            newProduct.Price = productDTO.Price;
            newProduct.CountOfLikes = productDTO.CountOfLikes;

            _context.Product.Add(newProduct);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("GetProductById", new {productId = newProduct.Id}, newProduct);
        }

        // Delete a product by Id
        [HttpDelete("{productId}")]
        public ActionResult DeleteProduct(int productId)
        {
            var product = _context.Product
                .Include(p => p.Reviews)    
                .Include(p => p.ShoppingCartProducts)
                .FirstOrDefault(p => p.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            // Detach Related Entities
            _context.Entry(product).State = EntityState.Detached;

            // Delete Relations
            foreach (var review in product.Reviews)
            {
                _context.Entry(review).State = EntityState.Detached;
            }

            foreach (var shoppingCartProduct in product.ShoppingCartProducts)
            {
                _context.Entry(shoppingCartProduct).State = EntityState.Detached;
            }
            foreach (var review in product.Reviews)
            {
                _context.Reviews.Remove(review);  // Delete every review explicitly
            }

            foreach (var shoppingCartProduct in product.ShoppingCartProducts)
            {
                _context.ShoppingCartProducts.Remove(shoppingCartProduct);  // Delete every cart element explicitly
            }

            // Delete Product
            _context.Product.Remove(product);
            _context.SaveChanges(); 

            return Ok($"The {product.Name} has been deleted");
        }

        //To edit a product
        [HttpPut("{productId}")]
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
                existingProduct.ListOfProductsId = 1;
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
    }
}
