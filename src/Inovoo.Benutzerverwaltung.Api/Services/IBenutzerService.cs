using Inovoo.Benutzerverwaltung.Contracts;

namespace Inovoo.Benutzerverwaltung.Api.Services;

public interface IBenutzerService
{
   Task<Seite<BenutzerDto>> SearchAsync(string? suche, int seite, int seitengroesse, CancellationToken cancellationToken);

    Task<BenutzerDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Speichert die Änderungen. Liefert <c>null</c>, wenn die Id nicht existiert.</summary>
    /// <exception cref="PersonalnummerVergebenException">Die Personalnummer ist schon vergeben.</exception>
    Task<BenutzerDto?> UpdateAsync(Guid id, BenutzerAktualisierenRequest request, CancellationToken cancellationToken);
}