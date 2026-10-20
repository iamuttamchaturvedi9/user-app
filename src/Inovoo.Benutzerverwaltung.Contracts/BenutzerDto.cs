namespace Inovoo.Benutzerverwaltung.Contracts;

/// <summary>Represents a user returned by the API.</summary>
public sealed record BenutzerDto(
    Guid Id,
    string? Anrede,
    string Vorname,
    string Nachname,
    string? Strasse,
    string? Plz,
    string? Stadt,
    string Benutzer,
    string Personalnummer,
    string? EMail,
    string? Geburtsdatum);
