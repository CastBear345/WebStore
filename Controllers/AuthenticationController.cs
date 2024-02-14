using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swagger.Models.ModelsDTO;
using System.Security.Claims;
using Swagger.Models;
using System.Net;
using WebStore.Services.Interfacies;

namespace Swagger.Controllers;

/// <summary>
/// Контроллер, отвечающий за авторизацию пользователей.
/// </summary>
[Route("api/authentication")]
[ApiController]
[AllowAnonymous]
public class AuthenticationController : ControllerBase
{
    protected APIResponse _response;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AuthenticationController> _logger;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="AuthenticationController"/>.
    /// </summary>
    /// <param name="dbContext">Контекст базы данных приложения.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="logger">Логгер.</param>
    public AuthenticationController(IUserRepository userRepository, ILogger<AuthenticationController> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
        try
        {
            _logger.LogInformation("Получен запрос на регистрацию.");

            bool isUserNameUnique = _userRepository.IsUniqueUser(model.UserName);
            if (!isUserNameUnique)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Имя пользователя уже занято");
                _logger.LogWarning("Имя пользователя уже занято.");
                return BadRequest(_response);
            }

            var user = await _userRepository.Register(model);
            if (user == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Ошибка при регистрации");
                _logger.LogError("Ошибка при регистрации.");
                return BadRequest(_response);
            }

            // После успешной регистрации выполняем вход пользователя
            var loginRequest = new LoginRequestDTO
            {
                UserName = model.UserName,
                Password = model.Password
            };

            var loginResponse = await _userRepository.Login(loginRequest);

            await HttpContext.SignInAsync(new ClaimsPrincipal(_userRepository.ClaimsIdentity(loginResponse)));

            // Возвращаем данные пользователя
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = loginResponse;
            _response.ErrorMessages.Add("Вы успешно зарегистрировались!");
            _logger.LogInformation("Пользователь успешно зарегистрировался.");
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка при регистрации пользователя.");
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Произошла внутренняя ошибка сервера при регистрации пользователя.");
            return StatusCode((int)HttpStatusCode.InternalServerError, _response);
        }
    }

    /// <summary>
    /// Вход пользователя.
    /// </summary>
    /// <param name="model">Модель запроса входа.</param>
    /// <returns>Результат операции входа.</returns>
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequestDTO model)
    {
        try
        {
            _logger.LogInformation("Получен запрос на вход.");

            var loginResponse = await _userRepository.Login(model);
            if (loginResponse is null)
            {
                _response.StatusCode = HttpStatusCode.Unauthorized;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Имя пользователя или пароль неверны");
                _logger.LogWarning("Имя пользователя или пароль неверны.");
                return Unauthorized(_response);
            }

            // Проверяем, если пользователь уже аутентифицирован, то сначала выходим
            if (User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("Получен запрос на выход.");
                await HttpContext.SignOutAsync();
            }

            await HttpContext.SignInAsync(new ClaimsPrincipal(_userRepository.ClaimsIdentity(loginResponse)));

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = loginResponse;
            _response.ErrorMessages.Add("Вы успешно вошли в аккаунт!");
            _logger.LogInformation("Пользователь успешно вошел в аккаунт.");
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка при входе пользователя.");
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Произошла внутренняя ошибка сервера при входе пользователя.");
            return StatusCode((int)HttpStatusCode.InternalServerError, _response);
        }
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
        try {
            _logger.LogInformation("Получен запрос на выход.");

            await HttpContext.SignOutAsync();

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.ErrorMessages.Add("Вы успешно вышли из аккаунта.");
            _logger.LogInformation("Пользователь успешно вышли из аккаунта.");
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка при выходе пользователя из системы.");
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Произошла внутренняя ошибка сервера при выходе пользователя.");
            return StatusCode((int)HttpStatusCode.InternalServerError, _response);
        }
    }
}
