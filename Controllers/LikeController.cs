using Microsoft.AspNetCore.Mvc;

namespace WebStore.Controllers;

[ApiController]
[Route("{productId}/likes")]
public class LikeController : ControllerBase
{
    private readonly ApplicationContext _context;
    public LikeController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult AddLike(int productId)
    {
        var product = _context.Product.FirstOrDefault(p => p.Id == productId);

        if (product == null)
        {
            return NotFound();
        }

        product.CountOfLikes++;

        _context.SaveChanges();

        return Created();
    }

    [HttpDelete]
    public IActionResult DeleteLike(int productId)
    {
        var product = _context.Product.FirstOrDefault(p => p.Id == productId);

        if (product == null)
        {
            return NotFound();
        }

        product.CountOfLikes--;

        _context.SaveChanges();

        return Accepted();
    }
}
