using System.ComponentModel.DataAnnotations;

namespace Swagger.Model;

/// <summary>
/// Представляет сущность пользователя.
/// </summary>
public class User
{
    #region Свойства

    /// <summary>
    /// Уникальный идентификатор пользователя.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    [StringLength(100)]
    public required string UserName { get; set; } = String.Empty;

    /// <summary>
    /// Хэш пароля пользователя.
    /// </summary>
    [StringLength(500)]
    public required string PasswordHash { get; set; } = String.Empty;

    /// <summary>
    /// Соль для хэширования пароля пользователя.
    /// </summary>
    [StringLength(500)]
    public string Salt { get; set; } = String.Empty;

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    [StringLength(200)]
    public string FirstName { get; set; } = String.Empty;

    /// <summary>
    /// Фамилия пользователя.
    /// </summary>
    [StringLength(200)]
    public string LastName { get; set; } = String.Empty;

    /// <summary>
    /// Дата создания пользователя.
    /// </summary>
    public DateTime DoC { get; set; }

    /// <summary>
    /// Адрес пользователя.
    /// </summary>
    [StringLength(600)]
    public string Address { get; set; } = String.Empty;

    /// <summary>
    /// Роли пользователя.
    /// </summary>
    public required string Roles { get; set; }

    #endregion
}
