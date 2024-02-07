namespace Swagger.Models.ModelsDTO;

/// <summary>
/// Представляет DTO для запроса регистрации пользователя.
/// </summary>
public class RegistrationRequestDTO
{
    #region Свойства

    /// <summary>
    /// Ник пользователя.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Фамилия пользователя.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Пароль, выбранный при регистрации.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Адрес, указанный при регистрации.
    /// </summary>
    public string Address { get; set; }

    #endregion
}