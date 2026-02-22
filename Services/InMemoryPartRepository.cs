using System.Collections.Concurrent;
using Pr1.MinWebService.Domain;

namespace Pr1.MinWebService.Services;

/// <summary>
/// хранилище запчастей в памяти процесса
/// </summary>
public sealed class InMemoryPartRepository : IPartRepository
{
    private readonly ConcurrentDictionary<Guid, CarPart> _parts = new();

    public IReadOnlyCollection<CarPart> GetAll()
        => _parts.Values
            .OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

    public CarPart? GetById(Guid id)
        => _parts.TryGetValue(id, out var part) ? part : null;

    public CarPart Create(string name, decimal price)
    {
        var id = Guid.NewGuid();
        var part = new CarPart(id, name, price);

        _parts[id] = part;
        return part;
    }
}
