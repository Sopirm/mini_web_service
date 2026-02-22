namespace Pr1.MinWebService.Domain;

/// <summary>
/// запрос на создание запчасти
/// </summary>
public sealed record CreatePartRequest(string Name, decimal Price);
