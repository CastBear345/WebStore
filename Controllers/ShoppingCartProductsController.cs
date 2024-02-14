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
        private readonly ApplicationDbContext _context;
        public ShoppingCartProductsController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("GetShoppingCartProducts")]
        public async Task<IActionResult> GetShoppingCartProducts(int shoppingCartId)
        {
            var user = HttpContext.User.Identity.Name;
            var currentUser = _context.Users.FirstOrDefault(u => u.UserName == user);

            var products = await _context.ShoppingCartProducts.
                Include(p=>p.Product).
                Where(p=>p.ShoppingCartId == shoppingCartId && p.ShoppingCarts.UserId == currentUser.Id).
                ToListAsync();

            if (products == null)
            {
                return NotFound();
            }

            return Ok(products);
        }

        [HttpPost("AddProductToShoppingCart")]
        public async Task<IActionResult> AddProductToShoppingCart(ShoppingCartProductsDTO shoppingCartProductDTO)
        {
            var user = HttpContext.User.Identity.Name;
            var currentUser = _context.Users.FirstOrDefault(u => u.UserName == user);

            var newShoppingCartProduct = new ShoppingCartProducts()
            {
                ProductId=shoppingCartProductDTO.ProductId,
                ShoppingCartId=shoppingCartProductDTO.ShoppingCartId,
                UserId = currentUser.Id,
            };
            _context.ShoppingCartProducts.Add(newShoppingCartProduct);
            await _context.SaveChangesAsync();


            return Ok($"Успешно добавлено в корзину");
        }



        [HttpDelete("DeleteShoppingCartProduct")]
        public async Task<IActionResult> DeleteShoppingCartProduct(int shoppingCartProductId)
        {
            var user = HttpContext.User.Identity.Name;
            var currentUser = _context.Users.FirstOrDefault(u => u.UserName == user);

            var shoppingCartProductToDelete = _context.ShoppingCartProducts
                .FirstOrDefault(p => p.Id == shoppingCartProductId && p.UserId == currentUser.Id);

            if (shoppingCartProductToDelete == null)
            {
                return NotFound();
            }

            // Delete Product
            _context.ShoppingCartProducts.Remove(shoppingCartProductToDelete);
            await _context.SaveChangesAsync();

            return Ok($"Продукт удален с корзины");
        }
    }
}
