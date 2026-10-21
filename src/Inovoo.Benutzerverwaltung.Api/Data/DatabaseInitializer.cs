using Microsoft.Data.Sqlite;

namespace Inovoo.Benutzerverwaltung.Api.Data;

/// <summary>
/// Legt beim Start die Tabellen <c>Benutzer</c> und <c>Bankverbindung</c> an
/// und fügt Beispieldaten ein, falls die Datenbank noch leer ist.
/// </summary>
public static class DatabaseInitializer
{
    private const string Schema = """
        CREATE TABLE IF NOT EXISTS Benutzer (
            Id TEXT PRIMARY KEY, Anrede TEXT, Vorname TEXT NOT NULL, Nachname TEXT NOT NULL,
            Strasse TEXT, PLZ TEXT, Stadt TEXT, Benutzer TEXT NOT NULL, Kennwort TEXT NOT NULL,
            Personalnummer TEXT NOT NULL, EMail TEXT, Geburtsdatum TEXT);
        CREATE TABLE IF NOT EXISTS Bankverbindung (
            Id TEXT PRIMARY KEY, BenutzerId TEXT NOT NULL REFERENCES Benutzer(Id),
            IBAN TEXT NOT NULL, BIC TEXT, Bank TEXT);
        CREATE INDEX IF NOT EXISTS IX_Benutzer_Nachname ON Benutzer(Nachname, Vorname);
        CREATE INDEX IF NOT EXISTS IX_Bankverbindung_BenutzerId ON Bankverbindung(BenutzerId);
        CREATE UNIQUE INDEX IF NOT EXISTS IX_Benutzer_Personalnummer ON Benutzer(Personalnummer);
        """;

    // Frei erfundene Beispieldaten.
    private const string Beispieldaten = """
        INSERT INTO Benutzer VALUES
          ('11111111-1111-1111-1111-111111111111', 'Frau', 'Anna', 'Berger', 'Hauptstrasse 1', '1010', 'Wien',
           'aberger', 'beispiel', 'P-0001', 'anna.berger@example.com', '1988-04-12'),
          ('22222222-2222-2222-2222-222222222222', 'Herr', 'Lukas', 'Maier', 'Ringweg 7', '4020', 'Linz',
           'lmaier', 'beispiel', 'P-0002', 'lukas.maier@example.com', '1992-11-03');
        INSERT INTO Bankverbindung VALUES
          ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '11111111-1111-1111-1111-111111111111',
           'AT611904300234573201', 'BKAUATWW', 'Bank Austria'),
          ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', '22222222-2222-2222-2222-222222222222',
           'DE89370400440532013000', 'COBADEFFXXX', 'Commerzbank');
        """;

    /// <summary>Erstellt das Schema und die Beispieldaten (nur wenn noch keine Benutzer existieren).</summary>
    public static void Initialize(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        Execute(connection, Schema);
        if (CountBenutzer(connection) == 0)
        {
            Execute(connection, Beispieldaten);
        }
    }

    private static void Execute(SqliteConnection connection, string sql)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private static long CountBenutzer(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Benutzer";
        return (long)command.ExecuteScalar()!;
    }
}
