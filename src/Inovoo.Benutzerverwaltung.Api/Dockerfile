# Build-Kontext: Repository-Root
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Build.props .
COPY src/Inovoo.Benutzerverwaltung.Contracts src/Inovoo.Benutzerverwaltung.Contracts
COPY src/Inovoo.Benutzerverwaltung.Api src/Inovoo.Benutzerverwaltung.Api
RUN dotnet publish src/Inovoo.Benutzerverwaltung.Api -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
RUN mkdir /data && chown $APP_UID /data
USER $APP_UID
ENTRYPOINT ["dotnet", "Inovoo.Benutzerverwaltung.Api.dll"]
