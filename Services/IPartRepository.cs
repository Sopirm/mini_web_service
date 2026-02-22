using Pr1.MinWebService.Domain;

namespace Pr1.MinWebService.Services;

/// <summary>
/// интерфейс хранилища запчастей
/// </summary>
public interface IPartRepository
{
    IReadOnlyCollection<CarPart> GetAll();

    CarPart? GetById(Guid id);

    CarPart Create(string name, decimal price);
}
