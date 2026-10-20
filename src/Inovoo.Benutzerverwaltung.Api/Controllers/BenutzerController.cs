using Inovoo.Benutzerverwaltung.Api.Services;
using Inovoo.Benutzerverwaltung.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Inovoo.Benutzerverwaltung.Api.Controllers;

[ApiController]
[Route("api/benutzer")]
public sealed class BenutzerController(IBenutzerService benutzerService) : ControllerBase
{
    /// <summary>Searches users by first name, last name, or full name.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<BenutzerDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BenutzerDto>>> Search(
        [FromQuery] string? suche,
        CancellationToken cancellationToken)
    {
        var benutzer = await benutzerService.SearchAsync(suche, cancellationToken);
        return Ok(benutzer);
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
}
