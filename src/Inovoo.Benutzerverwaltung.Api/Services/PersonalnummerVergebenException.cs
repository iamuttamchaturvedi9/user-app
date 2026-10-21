namespace Inovoo.Benutzerverwaltung.Api.Services;

/// <summary>Die Personalnummer gehört bereits einem anderen Benutzer.</summary>
public sealed class PersonalnummerVergebenException(string personalnummer, Exception inner)
    : Exception($"Die Personalnummer '{personalnummer}' ist bereits vergeben.", inner);

    