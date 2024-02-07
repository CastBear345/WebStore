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

    #endregion
}