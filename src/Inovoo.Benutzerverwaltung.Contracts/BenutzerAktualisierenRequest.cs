using System.ComponentModel.DataAnnotations;

namespace Inovoo.Benutzerverwaltung.Contracts;

/// <summary>
/// Änderbare Stammdaten eines Benutzers. Id, Benutzername und Kennwort sind bewusst nicht enthalten.
/// </summary>
public sealed class BenutzerAktualisierenRequest : IValidatableObject
{
    [StringLength(50)]
    public string? Anrede { get; init; }

    [Required(ErrorMessage = "Vorname ist erforderlich."), StringLength(50)]
    public string Vorname { get; init; } = "";

    [Required(ErrorMessage = "Nachname ist erforderlich."), StringLength(50)]
    public string Nachname { get; init; } = "";

    [StringLength(50)]
    public string? Strasse { get; init; }

    [StringLength(6, ErrorMessage = "PLZ darf höchstens 6 Zeichen haben.")]
    public string? Plz { get; init; }

    [StringLength(50)]
    public string? Stadt { get; init; }

    [Required(ErrorMessage = "Personalnummer ist erforderlich."), StringLength(50)]
    public string Personalnummer { get; init; } = "";

    [EmailAddress(ErrorMessage = "E-Mail-Adresse ist ungültig."), StringLength(50)]
    public string? Email { get; init; }

    /// <summary>Format <c>yyyy-MM-dd</c>.</summary>
    public string? Geburtsdatum { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(Geburtsdatum)
            && !DateOnly.TryParseExact(Geburtsdatum.Trim(), "yyyy-MM-dd", out _))
        {
            yield return new ValidationResult(
                "Geburtsdatum muss das Format yyyy-MM-dd haben.", [nameof(Geburtsdatum)]);
        }
    }
}