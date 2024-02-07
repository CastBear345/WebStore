using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Swagger.Models.ModelsDTO;
using Swagger.Models;
using System.Net;
using WebStore;

namespace Swagger.Controllers;

/// <summary>
/// Контроллер, отвечающий за управление текущим пользователем.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ApplicationContext _context;
    protected APIResponse _response;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="UserController"/>.
    /// </summary>
    /// <param name="context">Контекст базы данных приложения.</param>
    public UserController(ApplicationContext context)
    {
        _context = context;
        this._response = new();
    }

    /// <summary>
    /// Получает информацию о текущем пользователе.
    /// </summary>
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("GetCurrentUser")]
    [Authorize]
    public async Task<ActionResult> GetCurrentUser()
    {
        var currentUser = HttpContext.User.Identity.Name;
        var users = await _context.Users
            .Where(u => u.UserName == currentUser)
            .ToListAsync();

        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        _response.Result = users;
        return Ok(_response);
    }

    /// <summary>
    /// Удаляет текущего пользователя.
    /// </summary>
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpDelete("DeleteCurrentUser")]
    [Authorize]
    public async Task<ActionResult> DeleteCurrentUser()
    {
        var currentUser = HttpContext.User.Identity.Name;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == currentUser);

        if (user == null)
        {
            _response.StatusCode = HttpStatusCode.NotFound;
            _response.IsSuccess = false;
            return NotFound(_response);
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        _response.StatusCode = HttpStatusCode.NoContent;
        _response.IsSuccess = false;
        return Ok(_response);
    }

    /// <summary>
    /// Обновляет информацию о текущем пользователе.
    /// </summary>
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPut("UpdateCurrentUser")]
    [Authorize]
    public async Task<IActionResult> UpdateCurrentUser(RegistrationRequestDTO user)
    {
        var currentUser = HttpContext.User.Identity.Name;
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == currentUser);

        if (existingUser == null)
        {
            _response.StatusCode = HttpStatusCode.NotFound;
            _response.IsSuccess = false;
            return NotFound(_response);
        }

        try
        {
            _context.Entry(existingUser).CurrentValues.SetValues(user);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(existingUser.Id))
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }
            else
            {
                throw;
            }
        }

        _response.StatusCode = HttpStatusCode.NoContent;
        _response.IsSuccess = false;
        _response.Result = existingUser;
        return Ok(_response);
    }

    private bool UserExists(int id)
    {
        return _context.Users.Any(e => e.Id == id);
    }
}
