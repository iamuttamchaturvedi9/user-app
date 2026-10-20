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

    public async Task<IReadOnlyList<BenutzerDto>> SearchAsync(
        string? suche,
        CancellationToken cancellationToken)
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = BenutzerQuery + "\n" + """
            WHERE $search = ''
               OR Vorname LIKE $pattern
               OR Nachname LIKE $pattern
               OR Vorname || ' ' || Nachname LIKE $pattern
            ORDER BY Nachname, Vorname
            """;

        var search = suche?.Trim() ?? string.Empty;
        command.Parameters.AddWithValue("$search", search);
        command.Parameters.AddWithValue("$pattern", $"%{search}%");

        var benutzer = new List<BenutzerDto>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            benutzer.Add(ReadBenutzer(reader));
        }

        return benutzer;
    }

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
