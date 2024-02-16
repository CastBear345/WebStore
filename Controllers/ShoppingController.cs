using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebStore.Model.ModelDTO;
using WebStore.Model;

namespace WebStore.Controllers;

[Authorize(Roles = "User")]
[Route("api/shopping")]
[ApiController]
public class ShoppingController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    public ShoppingController(ApplicationDbContext context)
    {
        _dbContext = context;
    }

    [HttpGet("all-shopping-products")]
    public async Task<IActionResult> GetShoppingProducts()
    {
        var user = HttpContext.User.Identity.Name;
        var currentUser = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.UserName == user);
        var products = await _dbContext.ShoppingCartProducts.
                Include(p => p.Product).
                Where(p => p.ShoppingCarts.Name == "Shopping Busket" && p.ShoppingCarts.UserId == currentUser.Id).
                ToListAsync();

        if (products == null)
            return NotFound();

        return Ok(products);
    }

    [HttpPost("add-shopping-cart")]
    public async Task<IActionResult> AddShoppingCart(ShoppingCartsDTO shoppingCarts)
    {
        var user = HttpContext.User.Identity.Name;
        var currentUser = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.UserName == user);

        if (currentUser == null)
            return BadRequest();

        var shoppingProducts = await _dbContext.ShoppingCarts
            .Where(c => c.Name == "Shopping Busket" && c.UserId == currentUser.Id)
            .ToListAsync();

        if (shoppingProducts == null)
            return BadRequest();

        if (shoppingCarts.Name == null || shoppingCarts.Description == null)
            return BadRequest();
        
        await _dbContext.SaveChangesAsync();

        return Ok($"Корзинка {shoppingCarts.Name} успешно создано");
    }

    [HttpPut("update-shopping-cart")]
    public async Task<IActionResult> UpdateShoppingCart(ShoppingCartsDTO shoppingCart, int? shoppingCartId)
    {
        if (shoppingCart.Name == null || shoppingCart.Description == null || shoppingCartId == null)
            return BadRequest();

        var shoppingCartToUpdate = _dbContext.ShoppingCarts.FirstOrDefault(c => c.Id == shoppingCartId);

        if (shoppingCartToUpdate == null)
        {
            return NotFound();
        }

        shoppingCartToUpdate.Name = shoppingCart.Name;
        shoppingCartToUpdate.Description = shoppingCart.Description;

        await _dbContext.SaveChangesAsync();
        return Created();
    }

    [HttpDelete("delete-shopping-cart")]
    public async Task<IActionResult> DeleteShoppingCart(int shoppingCartId)
    {
        var shoppingCartToDelete = _dbContext.ShoppingCarts
            .Include(p => p.ShoppingCartProducts)
            .FirstOrDefault(p => p.Id == shoppingCartId);

        if (shoppingCartToDelete == null)
        {
            return NotFound();
        }

        // Detach Related Entities
        _dbContext.Entry(shoppingCartToDelete).State = EntityState.Detached;

        // Delete Relations

        foreach (var shoppingCartProduct in shoppingCartToDelete.ShoppingCartProducts)
        {
            _dbContext.Entry(shoppingCartProduct).State = EntityState.Detached;
        }

        foreach (var shoppingCartProduct in shoppingCartToDelete.ShoppingCartProducts)
        {
            _dbContext.ShoppingCartProducts.Remove(shoppingCartProduct);  // Delete every cart element explicitly
        }

        // Delete Product
        _dbContext.ShoppingCarts.Remove(shoppingCartToDelete);
        await _dbContext.SaveChangesAsync();

        return Ok($"The {shoppingCartToDelete.Name} has been deleted");
    }
}
