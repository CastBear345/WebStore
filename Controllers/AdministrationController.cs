using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Swagger.Models.ModelsDTO;
using Swagger.Repository;
using Swagger.Models;
using System.Net;
using WebStore;

namespace Swagger.Controllers;

/// <summary>
/// Контроллер, отвечающий за административные функции.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AdministrationController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ApplicationContext _context;
    protected APIResponse _response;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="AdministrationController"/>.
    /// </summary>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="context">Контекст базы данных приложения.</param>
    public AdministrationController(IUserRepository userRepository, ApplicationContext context)
    {
        _userRepository = userRepository;
        _context = context;
        this._response = new();
    }

    /// <summary>
    /// Получает список пользователей.
    /// </summary>
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("GetUsersList")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> GetUsersList()
    {
        var currentUser = HttpContext.User.Identity.Name;
        var users = await _context.Users
            .Where(u => u.UserName != currentUser)
            .ToListAsync();

        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        _response.Result = users;
        return Ok(_response);
    }

    /// <summary>
    /// Добавляет нового пользователя.
    /// </summary>
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("AddUser")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> AddUser([FromBody] RegistrationRequestDTO model)
    {
        bool isUserNameUnique = _userRepository.IsUniqueUser(model.UserName);
        if (!isUserNameUnique)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("UserName already exists");
            return BadRequest(_response);
        }

        var user = await _userRepository.Register(model);
        if (user == null)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Error while registering");
            return BadRequest(_response);
        }

        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        _response.Result = user;
        return Ok(_response);
    }

    /// <summary>
    /// Удаляет пользователя.
    /// </summary>
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpDelete("DeleteUser/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            _response.StatusCode = HttpStatusCode.NotFound;
            _response.IsSuccess = false;
            return NotFound(_response);
        }

        var currentUser = HttpContext.User.Identity.Name;
        if (user.UserName == currentUser)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("You cannot delete yourself.");
            return BadRequest(_response);
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        _response.StatusCode = HttpStatusCode.NoContent;
        _response.IsSuccess = false;
        return Ok(_response);
    }

    /// <summary>
    /// Обновляет информацию о пользователе.
    /// </summary>
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPut("UpdateUser/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUser(int id, UsersUpdateDTO user)
    {
        var existingUser = await _context.Users.FindAsync(id);
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
            if (!UserExists(id))
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