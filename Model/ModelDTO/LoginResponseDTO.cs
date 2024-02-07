using Swagger.Model;

namespace Swagger.Models.ModelsDTO;

/// <summary>
/// Представляет DTO (Data Transfer Object) для ответа на запрос аутентификации.
/// </summary>
public class LoginResponseDTO
{
    #region Свойства

    /// <summary>
    /// Пользователь, связанный с успешной аутентификацией.
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// Токен аутентификации, который может использоваться для последующих запросов.
    /// </summary>
    public string Token { get; set; }

    #endregion
}