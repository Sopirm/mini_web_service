namespace Pr1.MinWebService.Domain;

/// <summary>
/// автомобильная запчасть
/// </summary>
public sealed record CarPart(Guid Id, string Name, decimal Price);
