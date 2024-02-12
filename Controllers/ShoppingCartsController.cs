using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebStore.Model;
using WebStore.Model.ModelDTO;

namespace WebStore.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ShoppingCartsController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("GetShoppingCarts")]
        public async Task<IActionResult> GetShoppingCarts()
        {
            var shoppingCarts = _context.ShoppingCarts.ToList();
            return Ok(shoppingCarts);
        }


        [HttpPost("AddShoppingCart")]
        public async Task<IActionResult> AddShoppingCart(ShoppingCartsDTO shoppingCarts)
        {
            var newCategory = new ShoppingCarts()
            {
                Name = shoppingCarts.Name,
                Description = shoppingCarts.Description,
            };
            _context.ShoppingCarts.Add(newCategory);
            _context.SaveChanges();

            return Ok($"Корзинка {shoppingCarts.Name} успешно создано");
        }


        [HttpPut("UpdateShoppingCart")]
        public async Task<IActionResult> UpdateShoppingCart(ShoppingCartsDTO shoppingCart, int shoppingCartId)
        {
            var shoppingCartToUpdate = _context.ShoppingCarts.FirstOrDefault(c => c.Id == shoppingCartId);

            if (shoppingCartToUpdate == null)
            {
                return NotFound();
            }

            shoppingCartToUpdate.Name = shoppingCart.Name;
            shoppingCartToUpdate.Description = shoppingCart.Description;

            _context.SaveChanges();
            return Created();
        }


        [HttpDelete("DeleteShoppingCart")]
        public async Task<IActionResult> DeleteShoppingCart(int shoppingCartId)
        {
            var shoppingCartToDelete = _context.ShoppingCarts
                .Include(p => p.ShoppingCartProducts)
                .FirstOrDefault(p => p.Id == shoppingCartId);

            if (shoppingCartToDelete == null)
            {
                return NotFound();
            }

            // Detach Related Entities
            _context.Entry(shoppingCartToDelete).State = EntityState.Detached;

            // Delete Relations

            foreach (var shoppingCartProduct in shoppingCartToDelete.ShoppingCartProducts)
            {
                _context.Entry(shoppingCartProduct).State = EntityState.Detached;
            }

            foreach (var shoppingCartProduct in shoppingCartToDelete.ShoppingCartProducts)
            {
                _context.ShoppingCartProducts.Remove(shoppingCartProduct);  // Delete every cart element explicitly
            }

            // Delete Product
            _context.ShoppingCarts.Remove(shoppingCartToDelete);
            _context.SaveChanges();

            return Ok($"The {shoppingCartToDelete.Name} has been deleted");
        }
    }
}
