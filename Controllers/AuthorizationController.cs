using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swagger.Models.ModelsDTO;
using System.Security.Claims;
using Swagger.Repository;
using Swagger.Models;
using System.Net;

namespace Swagger.Controllers;

/// <summary>
/// Контроллер, отвечающий за авторизацию пользователей.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthorizationController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    protected APIResponse _response;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="AuthorizationController"/>.
    /// </summary>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    public AuthorizationController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        this._response = new();
    }

    /// <summary>
    /// Регистрация нового пользователя.
    /// </summary>
    /// <param name="model">Модель запроса регистрации.</param>
    /// <returns>Результат операции регистрации.</returns>
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegistrationRequestDTO model)
    {
        bool isUserNameUnique = _userRepository.IsUniqueUser(model.UserName);
        if (!isUserNameUnique)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Имя пользователя уже занято");
            return BadRequest(_response);
        }

        var user = await _userRepository.Register(model);
        if (user == null)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Ошибка при регистрации");
            return BadRequest(_response);
        }

        // После успешной регистрации выполняем вход пользователя
        var loginRequest = new LoginRequestDTO
        {
            UserName = model.UserName,
            Password = model.Password
        };

        var loginResponse = await _userRepository.Login(loginRequest);

        await HttpContext.SignInAsync(new ClaimsPrincipal(ClaimsIdentity(loginResponse)));

        // Возвращаем данные пользователя
        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        _response.Result = loginResponse;
        return Ok(_response);
    }

    /// <summary>
    /// Вход пользователя.
    /// </summary>
    /// <param name="model">Модель запроса входа.</param>
    /// <returns>Результат операции входа.</returns>
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequestDTO model)
    {
        var loginResponse = await _userRepository.Login(model);
        if (loginResponse.User is null)
        {
            _response.StatusCode = HttpStatusCode.Unauthorized;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Имя пользователя или пароль неверны");
            return Unauthorized(_response);
        }

        await HttpContext.SignInAsync(new ClaimsPrincipal(ClaimsIdentity(loginResponse)));

        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        _response.Result = loginResponse;
        return Ok(_response);
    }

    /// <summary>
    /// Выход пользователя.
    /// </summary>
    /// <param name="model">Модель запроса выхода.</param>
    /// <returns>Результат операции выхода.</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        await HttpContext.SignOutAsync();

        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        _response.ErrorMessages.Add("Вы успешно вышли из аккаунта.");
        return Ok(_response);
    }

    private static ClaimsIdentity ClaimsIdentity(LoginResponseDTO loginResponse)
    {
        var claims = new List<Claim>() {
            new(ClaimTypes.NameIdentifier, loginResponse.User.Id.ToString()),
            new(ClaimTypes.Name, loginResponse.User.UserName),
            new(ClaimTypes.Role, loginResponse.User.Roles.ToString()),
            new(ClaimTypes.GivenName, loginResponse.User.FirstName + " " + loginResponse.User.LastName),
        };

        var claimsIdentity = new ClaimsIdentity(claims, "cookie");
        return claimsIdentity;
    }
}
