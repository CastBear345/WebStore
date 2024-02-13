using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebStore.Controllers;

/// <summary>
///     Контроллер управляющий лайками продуктов
/// </summary>
[ApiController]
[Authorize(Roles = "User")]
[Route("api/{productId}/likes")]
public class LikeController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    ///     Инициализирует контекст базы данных <see cref="ApplicationDbContext"/>
    /// </summary>
    /// <param name="context">Ссылка на контекст базы данных с помощю иньекции зависимостей</param>
    public LikeController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Добавление лайка в продукт
    /// </summary>
    /// <param name="productId">Идентификатор продукта</param>
    /// <returns>
    ///     Ошибку 404 если продукт не найден
    ///     Успех 201 если лайк успешно добавлен
    /// </returns>
    [HttpPost]
    public IActionResult AddLike(int productId)
    {
        var user = HttpContext.User.Identity.Name;
        var currentUser = _context.Users.FirstOrDefault(u => u.UserName == user);
        var product = _context.Product.FirstOrDefault(p => p.Id == productId);

        if (product == null || currentUser == null)
            return NotFound();

        if (product.LikedUserIds.Any(id => id == currentUser.Id))
            return BadRequest();

        product.CountOfLikes++;

        _context.SaveChanges();

        return Created();
    }

    /// <summary>
    ///     Удаление лайка из продукта
    /// </summary>
    /// <param name="productId">Идентификатор продукта</param>
    /// <returns>
    ///     Ошибку 404 если продукт не найден
    ///     Успех 202 если лайк успешно удален
    /// </returns>
    [HttpDelete]
    public IActionResult DeleteLike(int productId)
    {
        var user = HttpContext.User.Identity.Name;
        var currentUser = _context.Users.FirstOrDefault(u => u.UserName == user);
        var product = _context.Product.FirstOrDefault(p => p.Id == productId);

        if (product == null || currentUser == null)
            return NotFound();

        if (product.LikedUserIds.Any(id => id != currentUser.Id))
            return BadRequest();

        product.CountOfLikes--;

        _context.SaveChanges();

        return Accepted();
    }
}
