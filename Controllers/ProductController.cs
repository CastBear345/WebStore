using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using WebStore.Model.ModelDTO;
using WebStore.Model;
using Microsoft.AspNetCore.Authorization;

namespace WebStore.Controllers
{
    /// <summary>
    ///     Контроллер управляющий продуктами
    /// </summary>
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        ///     Инициализирует контекст базы данных <see cref="ApplicationDbContext"/>
        /// </summary>
        /// <param name="context">Ссылка на контекст базы данных с помощю иньекции зависимостей</param>
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        ///     Получение списка продуктов по идентификатору подкатегории
        /// </summary>
        /// <param name="subcategoryId">Идентификатор подкатегории</param>
        /// <returns>
        ///     Ошибку 404 если подкатегория продуктов не найдена
        ///     Успех 200 со списком продуктов если подкатегория найдена
        /// </returns>
        [HttpGet("{subcategoryId}")]
        public ActionResult<List<Product>> GetProductsBySubCategory(int subcategoryId, SortByEnum sortBy = SortByEnum.ByName, string search = "")
        {
            if (_context.SubCategory.FirstOrDefault(s => s.Id == subcategoryId) == null)
            {
                return NotFound();
            }

            var products = _context.Product.Where(c => c.SubCategoryId == subcategoryId).ToList();

            // Сортировка по SortByEnum
            switch (sortBy)
            {
                case SortByEnum.ByName:
                    products = products.OrderBy(c => c.Name).ToList();
                    break;
                case SortByEnum.ByPrice:
                    products = products.OrderBy(c => c.Price).ToList();
                    break;
                case SortByEnum.ByRating:
                    products = products.OrderBy(c => c.CountOfLikes).ToList();
                    break;
            }

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search)).ToList();
            }

            return Ok(products);
        }

        /// <summary>
        ///     Получение продукта
        /// </summary>
        /// <param name="productId">Идентификатор продукта</param>
        /// <returns>
        ///     Ошибку 404 если продукт не найден
        ///     Успех 200 с продуктом если он найден
        /// </returns>
        [HttpGet("product/{productId}", Name = "GetProductById")]
        public async Task<ActionResult<Product>> GetProduct(int productId)
        {
            var product = await _context.Product.FindAsync(productId);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        /// <summary>
        ///     Добавление продукта только для админа
        /// </summary>
        /// <param name="productDTO">Создаваемый объект продукта</param>
        /// <returns>
        ///     Успех 201 с маршрутом где можно его получить
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
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
            newProduct.Description = productDTO.Description;
            newProduct.Price = productDTO.Price;
            newProduct.CountOfLikes = productDTO.CountOfLikes;

            _context.Product.Add(newProduct);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("GetProductById", new {productId = newProduct.Id}, newProduct);
        }

        /// <summary>
        ///     Удаление продукта только для админа
        /// </summary>
        /// <param name="productId">Идентификатор продукта</param>
        /// <returns>
        ///     Ошибку 404 если продукт не найден
        ///     Успех 202 если продукт успешно удалён
        /// </returns>
        [HttpDelete("{productId}")]
        [Authorize(Roles = "Admin")]
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

            _context.Entry(product).State = EntityState.Detached;

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
                _context.Reviews.Remove(review);
            }
            foreach (var shoppingCartProduct in product.ShoppingCartProducts)
            {
                _context.ShoppingCartProducts.Remove(shoppingCartProduct);
            }

            _context.Product.Remove(product);
            _context.SaveChanges();

            return Accepted($"The {product.Name} has been deleted");
        }

        /// <summary>
        ///     Обновление продукта только для админа
        /// </summary>
        /// <param name="updatedProductDTO">объект с обновлёнными данными продукта</param>
        /// <returns>
        ///     Ошибка 404 если обновляемый продукт не найден
        ///     Успех 201 с маршрутом где можно его получить
        /// </returns>
        [HttpPut("{productId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditProduct(int productId, [FromBody] ProductDTO updatedProductDTO)
        {
            try
            {
                var existingProduct = await _context.Product.FindAsync(productId);

                if (existingProduct == null)
                {
                    return NotFound();
                }

                // Обновление свойств продукта
                existingProduct.Name = updatedProductDTO.Name;
                existingProduct.ImageURL = updatedProductDTO.ImageURL;
                existingProduct.SubCategoryId = updatedProductDTO.SubCategoryId;
                existingProduct.Description = updatedProductDTO.Description;
                existingProduct.Price = updatedProductDTO.Price;
                existingProduct.CountOfLikes = updatedProductDTO.CountOfLikes;


                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
