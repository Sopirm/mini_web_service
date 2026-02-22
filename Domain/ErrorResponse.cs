namespace Pr1.MinWebService.Domain;

/// <summary>
/// единый формат ошибки для клиентов
/// </summary>
public sealed record ErrorResponse(string Code, string Message, string RequestId);
