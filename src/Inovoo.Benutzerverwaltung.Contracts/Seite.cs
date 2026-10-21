namespace Inovoo.Benutzerverwaltung.Contracts;

/// <summary>Eine Ergebnisseite einer Suche samt Gesamtanzahl aller Treffer.</summary>
/// <typeparam name="T">Typ der Einträge.</typeparam>
public sealed record Seite<T>(IReadOnlyList<T> Eintraege, int Gesamtanzahl);