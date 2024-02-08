﻿using Microsoft.IdentityModel.Tokens;
using Swagger.Models.ModelsDTO;
using System.Security.Claims;
using Swagger.Model;
using System.Text;
using WebStore;
using Swagger.Models;

namespace Swagger.Repository;

/// <summary>
/// Репозиторий пользователей, реализующий интерфейс <see cref="IUserRepository"/>.
/// </summary>
public class UserRepository : IUserRepository
{
    #region Поля

    private readonly ApplicationDbContext _context;
    protected APIResponse _response;

    #endregion

    #region Конструктор

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="UserRepository"/>.
    /// </summary>
    /// <param name="context">Контекст базы данных приложения.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #endregion

    #region Методы

    /// <summary>
    /// Проверяет, уникально ли имя пользователя.
    /// </summary>
    /// <param name="username">Имя пользователя для проверки.</param>
    /// <returns>True, если имя пользователя уникально, иначе - false.</returns>
    public bool IsUniqueUser(string username)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName == username);

        if (user == null)
            return true;

        return false;
    }

    /// <summary>
    /// Выполняет вход пользователя.
    /// </summary>
    /// <param name="loginRequestDTO">DTO для запроса входа.</param>
    /// <returns>DTO ответа на запрос входа.</returns>
    public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
    {
        var user = _context.Users.FirstOrDefault(u =>
            u.UserName.ToLower() == loginRequestDTO.UserName.ToLower()
        );

        if (user != null)
        {
            // Используем соль из базы данных для проверки пароля
            bool isPasswordValid = BCrypt.Net.BCrypt.HashPassword(loginRequestDTO.Password, user.Salt) == user.PasswordHash;

            if (isPasswordValid)
            {

                var loginResponseDTO = new LoginResponseDTO()
                {
                    User = user,
                };

                return loginResponseDTO;
            }
        }

        // Если пользователь не найден или пароль неверный, возвращаем пустой токен и null вместо данных пользователя
        return new LoginResponseDTO
        {
            User = null
        };
    }

    /// <summary>
    /// Регистрирует нового пользователя.
    /// </summary>
    /// <param name="registrationRequestDTO">DTO для запроса регистрации.</param>
    /// <returns>Зарегистрированный пользователь.</returns>
    public async Task<User> Register(RegistrationRequestDTO registrationRequestDTO)
    {
        string salt = BCrypt.Net.BCrypt.GenerateSalt();
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(registrationRequestDTO.Password, salt);
        User user = new()
        {
            UserName = registrationRequestDTO.UserName,
            PasswordHash = passwordHash,
            Salt = salt,
            FirstName = registrationRequestDTO.FirstName,
            LastName = registrationRequestDTO.LastName,
            Address = registrationRequestDTO.Address,
            DoC = DateTime.Now.AddDays(5),
            Roles = "User",
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    #endregion
}
