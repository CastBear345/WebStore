﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebStore.Model;
using WebStore.Model.ModelDTO;

namespace WebStore.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/shopping-carts")]
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
            var user = HttpContext.User.Identity.Name;
            var currentUser = _context.Users.FirstOrDefault(u => u.UserName == user);

            var shoppingCarts = await _context.ShoppingCarts
                .Where(u => u.UserId == currentUser.Id)
                .ToListAsync();

            if (shoppingCarts == null)
                return BadRequest();

            return Ok(shoppingCarts);
        }

        [HttpPost("AddShoppingCart")]
        public async Task<IActionResult> AddShoppingCart(ShoppingCartsDTO shoppingCarts)
        {
            var user = HttpContext.User.Identity.Name;
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == user);
            var existingShoppingCarts = _context.ShoppingCarts.Where(s => s.Name == shoppingCarts.Name);

            if (shoppingCarts.Name == null || shoppingCarts.Description == null || existingShoppingCarts != null)
                return BadRequest();

            var newCategory = new ShoppingCarts()
            {
                Name = shoppingCarts.Name,
                Description = shoppingCarts.Description,
                UserId = currentUser.Id,
            };

            _context.ShoppingCarts.Add(newCategory);
            await _context.SaveChangesAsync();

            return Ok($"Корзинка {shoppingCarts.Name} успешно создано");
        }


        [HttpPut("UpdateShoppingCart")]
        public async Task<IActionResult> UpdateShoppingCart(ShoppingCartsDTO shoppingCart, int? shoppingCartId)
        {
            if (shoppingCart.Name == null || shoppingCart.Description == null || shoppingCartId == null)
                return BadRequest();

            var shoppingCartToUpdate = _context.ShoppingCarts.FirstOrDefault(c => c.Id == shoppingCartId);

            if (shoppingCartToUpdate == null)
            {
                return NotFound();
            }

            shoppingCartToUpdate.Name = shoppingCart.Name;
            shoppingCartToUpdate.Description = shoppingCart.Description;

            await _context.SaveChangesAsync();
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
            await _context.SaveChangesAsync();

            return Ok($"The {shoppingCartToDelete.Name} has been deleted");
        }
    }
}
