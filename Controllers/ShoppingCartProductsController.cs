using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebStore.Model.ModelDTO;
using WebStore.Model;

namespace WebStore.Controllers
{
    [Route("api/[controller]")]
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
            var products = _context.ShoppingCartProducts.
                Include(p=>p.Product).
                Where(p=>p.ShoppingCartId == shoppingCartId).
                ToList();
                ;
            return Ok(products);
        }


        [HttpPost("AddProductToShoppingCart")]
        public async Task<IActionResult> AddProductToShoppingCart(ShoppingCartProductsDTO shoppingCartProductDTO)
        {
            var newShoppingCartProduct = new ShoppingCartProducts()
            {
                ProductId=shoppingCartProductDTO.ProductId,
                ShoppingCartId=shoppingCartProductDTO.ShoppingCartId,
                
            };
            _context.ShoppingCartProducts.Add(newShoppingCartProduct);
            _context.SaveChanges();


            return Ok($"Успешно добавлено в корзину");
        }



        [HttpDelete("DeleteShoppingCartProduct")]
        public async Task<IActionResult> DeleteShoppingCartProduct(int shoppingCartProductId)
        {
            var shoppingCartProductToDelete = _context.ShoppingCartProducts
                .FirstOrDefault(p => p.Id == shoppingCartProductId);

            if (shoppingCartProductToDelete == null)
            {
                return NotFound();
            }

            // Delete Product
            _context.ShoppingCartProducts.Remove(shoppingCartProductToDelete);
            _context.SaveChanges();

            return Ok($"Продукт удален с корзины");
        }
    }
}
