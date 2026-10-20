using Inovoo.Benutzerverwaltung.Contracts;

namespace Inovoo.Benutzerverwaltung.Api.Services;

public interface IBenutzerService
{
    Task<IReadOnlyList<BenutzerDto>> SearchAsync(string? suche, CancellationToken cancellationToken);

    Task<BenutzerDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
