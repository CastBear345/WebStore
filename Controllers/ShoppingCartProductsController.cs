using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebStore.Model.ModelDTO;
using WebStore.Model;
using Microsoft.AspNetCore.Authorization;

namespace WebStore.Controllers
{
    [Route("api/shopping-cart-products")]
    [Authorize(Roles = "User")]
    [ApiController]
    public class ShoppingCartProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public ShoppingCartProductsController(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        [HttpGet("{shoppingCartName}/all-products-shopping-cart")]
        public async Task<IActionResult> GetShoppingCartProducts(string shoppingCartName)
        {
            var user = HttpContext.User.Identity.Name;
            var currentUser = _dbContext.Users.FirstOrDefault(u => u.UserName == user);

            var products = await _dbContext.ShoppingCartProducts.
                Include(p=>p.Product).
                Where(p=> p.ShoppingCarts.UserId == currentUser.Id && p.ShoppingCarts.Name == shoppingCartName).
                ToListAsync();

            if (products == null)
            {
                return NotFound();
            }

            return Ok(products);
        }

        [HttpPost("add-product-shopping-cart")]
        public async Task<IActionResult> AddProductToShoppingCart(ShoppingCartProductsDTO shoppingCartProductDTO)
        {
            var user = HttpContext.User.Identity.Name;
            var currentUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == user);
            var cart = await _dbContext.ShoppingCarts.FirstOrDefaultAsync(s => s.UserId == currentUser.Id && s.Name == shoppingCartProductDTO.ShoppingCartName);
            var product = await _dbContext.Product.FindAsync(shoppingCartProductDTO.ProductId);

            if (product == null)
            {
                return NotFound($"Продукт с id {shoppingCartProductDTO.ProductId} не найден");
            }

            // Проверяем, есть ли уже такой продукт в корзине пользователя
            var existingItem = await _dbContext.ShoppingCartProducts
                .FirstOrDefaultAsync(p => p.ProductId == product.Id && p.UserId == currentUser.Id);

            if (existingItem != null)
            {
                // Если продукт уже есть в корзине, обновляем количество
                existingItem.Quantity += shoppingCartProductDTO.ProductQuantity;
            }
            else
            {
                // Иначе добавляем новый продукт в корзину
                var newShoppingCartProduct = new ShoppingCartProducts()
                {
                    ProductId = product.Id,
                    ShoppingCartId = cart.Id,
                    Quantity = shoppingCartProductDTO.ProductQuantity,
                    UserId = currentUser.Id,
                };
                _dbContext.ShoppingCartProducts.Add(newShoppingCartProduct);
            }

            await _dbContext.SaveChangesAsync();

            return Ok($"{product.Name} успешно добавлено в корзину");
        }


        [HttpDelete("{shoppingCartProductId}/del-product-shopping-cart")]
        public async Task<IActionResult> DeleteShoppingCartProduct(int shoppingCartProductId)
        {
            var user = HttpContext.User.Identity.Name;
            var currentUser = _dbContext.Users.FirstOrDefault(u => u.UserName == user);

            var shoppingCartProductToDelete = _dbContext.ShoppingCartProducts
                .FirstOrDefault(p => p.Id == shoppingCartProductId && p.UserId == currentUser.Id);

            if (shoppingCartProductToDelete == null)
            {
                return NotFound();
            }

            // Delete Product
            _dbContext.ShoppingCartProducts.Remove(shoppingCartProductToDelete);
            await _dbContext.SaveChangesAsync();

            return Ok($"Продукт удален с корзины");
        }

        [HttpPut("{shoppingCartProductId}/upd-product-shopping-cart")]
        public async Task<IActionResult> ChangeProductToShoppingCart(int shoppingCartProductId, ShoppingCartProductsDTO shoppingCartProductDTO)
        {
            var user = HttpContext.User.Identity.Name;
            var currentUser = _dbContext.Users.FirstOrDefault(u => u.UserName == user);

            var shoppingCartProductToChange = _dbContext.ShoppingCartProducts
                .FirstOrDefault(p => p.Id == shoppingCartProductId && p.UserId == currentUser.Id);

            if (shoppingCartProductToChange == null)
                return NotFound();

            shoppingCartProductToChange.Quantity = shoppingCartProductDTO.ProductQuantity;
            await _dbContext.SaveChangesAsync();

            return Ok($"Продукт успешно изменен");
        }
    }
}
