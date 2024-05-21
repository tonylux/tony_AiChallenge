# Étape de construction 1: Construire l'application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

# Copier le fichier .csproj du projet spécifique et restaurer les dépendances
COPY Auctions/Auctions.csproj ./
RUN dotnet restore Auctions.csproj

# Copier le code source et construire l'application
COPY . .
RUN dotnet publish -c release -o /app

# Étape de construction 2: Générer l'image de runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

# Copier les fichiers de l'étape de construction 1
COPY --from=build /app .

# Définir les variables d'environnement
ENV ASPNETCORE_URLS=http://+:80 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    ASPNETCORE_ENVIRONMENT=Production

# Exposer le port 80
EXPOSE 80

# Définir la commande pour démarrer l'application
ENTRYPOINT ["dotnet", "Auctions.dll"]



