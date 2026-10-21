using Inovoo.Benutzerverwaltung.Contracts;
using Microsoft.Data.Sqlite;

namespace Inovoo.Benutzerverwaltung.Api.Services;

public sealed class BenutzerService(string connectionString) : IBenutzerService
{
    private const string BenutzerQuery = """
        SELECT Id, Anrede, Vorname, Nachname, Strasse, PLZ, Stadt, Benutzer,
               Personalnummer, EMail, Geburtsdatum
        FROM Benutzer
        """;

    private const string SuchFilter = """
    WHERE $muster IS NULL
       OR Vorname LIKE $muster ESCAPE '\' OR Nachname LIKE $muster ESCAPE '\'
       OR Vorname || ' ' || Nachname LIKE $muster ESCAPE '\'
       OR Personalnummer LIKE $muster ESCAPE '\' OR EMail LIKE $muster ESCAPE '\'
       OR Stadt LIKE $muster ESCAPE '\' OR Benutzer LIKE $muster ESCAPE '\'
    """;

    public async Task<Seite<BenutzerDto>> SearchAsync(
        string? suche, int seite, int seitengroesse, CancellationToken cancellationToken)
    {
        var muster = ZuSuchmuster(suche);
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var zaehlen = connection.CreateCommand();
        zaehlen.CommandText = "SELECT COUNT(*) FROM Benutzer " + SuchFilter;
        zaehlen.Parameters.AddWithValue("$muster", (object?)muster ?? DBNull.Value);
        var gesamt = (long)(await zaehlen.ExecuteScalarAsync(cancellationToken))!;

        await using var command = connection.CreateCommand();
        command.CommandText = BenutzerQuery + "\n" + SuchFilter
            + "\nORDER BY Nachname, Vorname LIMIT $limit OFFSET $offset";
        command.Parameters.AddWithValue("$muster", (object?)muster ?? DBNull.Value);
        command.Parameters.AddWithValue("$limit", seitengroesse);
        command.Parameters.AddWithValue("$offset", (seite - 1) * seitengroesse);

        var eintraege = new List<BenutzerDto>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            eintraege.Add(ReadBenutzer(reader));
        }

        return new Seite<BenutzerDto>(eintraege, (int)gesamt);
    }

    /// <summary>Macht aus dem Suchtext ein LIKE-Muster; Platzhalter im Text (% _ \) werden maskiert.</summary>
    private static string? ZuSuchmuster(string? suche) =>
        string.IsNullOrWhiteSpace(suche)
            ? null
            : "%" + suche.Trim().Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_") + "%";

    public async Task<BenutzerDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = BenutzerQuery + " WHERE Id = $id";
        command.Parameters.AddWithValue("$id", id.ToString());

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? ReadBenutzer(reader) : null;
    }

    private const int SqliteConstraintErrorCode = 19;

    public async Task<BenutzerDto?> UpdateAsync(
        Guid id, BenutzerAktualisierenRequest request, CancellationToken cancellationToken)
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
        UPDATE Benutzer
        SET Anrede = $anrede, Vorname = $vorname, Nachname = $nachname, Strasse = $strasse,
            PLZ = $plz, Stadt = $stadt, Personalnummer = $personalnummer,
            EMail = $email, Geburtsdatum = $geburtsdatum
        WHERE Id = $id
        """;
        command.Parameters.AddWithValue("$id", id.ToString());
        command.Parameters.AddWithValue("$anrede", Wert(request.Anrede));
        command.Parameters.AddWithValue("$vorname", Wert(request.Vorname));
        command.Parameters.AddWithValue("$nachname", Wert(request.Nachname));
        command.Parameters.AddWithValue("$strasse", Wert(request.Strasse));
        command.Parameters.AddWithValue("$plz", Wert(request.Plz));
        command.Parameters.AddWithValue("$stadt", Wert(request.Stadt));
        command.Parameters.AddWithValue("$personalnummer", Wert(request.Personalnummer));
        command.Parameters.AddWithValue("$email", Wert(request.Email));
        command.Parameters.AddWithValue("$geburtsdatum", Wert(request.Geburtsdatum));

        int geaenderteZeilen;
        try
        {
            geaenderteZeilen = await command.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == SqliteConstraintErrorCode)
        {
            throw new PersonalnummerVergebenException(request.Personalnummer, ex);
        }

        return geaenderteZeilen == 0 ? null : await GetByIdAsync(id, cancellationToken);
    }

    /// <summary>Leere Texte werden als NULL gespeichert, sonst wird der Text getrimmt.</summary>
    private static object Wert(string? text) =>
        string.IsNullOrWhiteSpace(text) ? DBNull.Value : text.Trim();

    private static BenutzerDto ReadBenutzer(SqliteDataReader reader) => new(
        Guid.Parse(reader.GetString(0)),
        reader.IsDBNull(1) ? null : reader.GetString(1),
        reader.GetString(2),
        reader.GetString(3),
        reader.IsDBNull(4) ? null : reader.GetString(4),
        reader.IsDBNull(5) ? null : reader.GetString(5),
        reader.IsDBNull(6) ? null : reader.GetString(6),
        reader.GetString(7),
        reader.GetString(8),
        reader.IsDBNull(9) ? null : reader.GetString(9),
        reader.IsDBNull(10) ? null : reader.GetString(10));
}
