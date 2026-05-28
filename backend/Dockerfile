# backend/Dockerfile — image multi-stage de l'API .NET 8
# IMPORTANT : le contexte de build est ./backend (défini dans docker-compose.yml)
# car le projet CesiZen.Domain se trouve dans backend/ et est référencé par
# CesiZen.Infrastructure -> il doit être présent dans le contexte.

# ---------- Étape 1 : build & publish ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copie de l'ensemble du dossier backend (Domain + Infrastructure + Api)
COPY . .

# Restauration puis publication de l'API (les références projet sont résolues)
RUN dotnet restore "CesiZen.Api/CesiZen.Api/CesiZen.Api.csproj"
RUN dotnet publish "CesiZen.Api/CesiZen.Api/CesiZen.Api.csproj" \
    -c Release -o /app/publish /p:UseAppHost=false

# ---------- Étape 2 : image d'exécution allégée ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# L'API écoute en HTTP sur 8080 (TLS terminé par Nginx en amont)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "CesiZen.Api.dll"]
