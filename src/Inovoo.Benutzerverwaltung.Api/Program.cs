using Inovoo.Benutzerverwaltung.Api.Data;
using Inovoo.Benutzerverwaltung.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("The 'Default' connection string is not configured.");
DatabaseInitializer.Initialize(connectionString);
builder.Services.AddScoped<IBenutzerService>(_ => new BenutzerService(connectionString));

var app = builder.Build();
app.MapOpenApi();
app.MapHealthChecks("/health");
app.MapControllers();
app.Run();
