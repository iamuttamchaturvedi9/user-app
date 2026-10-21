using Inovoo.Benutzerverwaltung.Api.Services;
using Inovoo.Benutzerverwaltung.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Inovoo.Benutzerverwaltung.Api.Controllers;

[ApiController]
[Route("api/benutzer")]
public sealed class BenutzerController(IBenutzerService benutzerService) : ControllerBase
{   
    private const int MaxSeitengroesse = 100; 

    /// <summary>Sucht Benutzer (Name, Personalnummer, E-Mail, Stadt, Benutzername), seitenweise.</summary>
    [HttpGet]
    [ProducesResponseType<Seite<BenutzerDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<Seite<BenutzerDto>>> Search(
        [FromQuery] string? suche,
        [FromQuery] int seite = 1,
        [FromQuery] int seitengroesse = 20,
        CancellationToken cancellationToken = default)
    {
        var seitennummer = Math.Max(seite, 1);
        var groesse = Math.Clamp(seitengroesse, 1, MaxSeitengroesse);
        return Ok(await benutzerService.SearchAsync(suche, seitennummer, groesse, cancellationToken));
    }

    /// <summary>Gets a user by ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<BenutzerDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BenutzerDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var benutzer = await benutzerService.GetByIdAsync(id, cancellationToken);
        return benutzer is null ? NotFound() : Ok(benutzer);
    }

    /// <summary>Aktualisiert die Stammdaten eines Benutzers.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType<BenutzerDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BenutzerDto>> Update(
        Guid id, BenutzerAktualisierenRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var benutzer = await benutzerService.UpdateAsync(id, request, cancellationToken);
            return benutzer is null ? NotFound() : Ok(benutzer);
        }
        catch (PersonalnummerVergebenException ex)
        {
            return Conflict(new ProblemDetails { Title = ex.Message, Status = StatusCodes.Status409Conflict });
        }
    }
}
