using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebStore.Controllers;

/// <summary>
///     Контроллер управляющий лайками продуктов
/// </summary>
[ApiController]
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
    [Authorize]
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

    /// <summary>
    ///     Удаление лайка из продукта
    /// </summary>
    /// <param name="productId">Идентификатор продукта</param>
    /// <returns>
    ///     Ошибку 404 если продукт не найден
    ///     Успех 202 если лайк успешно удален
    /// </returns>
    [HttpDelete]
    [Authorize]
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
