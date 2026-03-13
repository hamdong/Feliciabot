# Use the official .NET 8 SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy project file and restore dependencies
COPY Feliciabot/Feliciabot.csproj ./Feliciabot/
RUN dotnet restore ./Feliciabot/Feliciabot.csproj

# Copy all source code
COPY . .

# Build and publish the app to /app/publish folder
RUN dotnet publish ./Feliciabot/Feliciabot.csproj -c Release -o /app/publish

# Use the official .NET 8 runtime image for the final lightweight container
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Copy published app from build stage
COPY --from=build /app/publish .

# Set the entry point (adjust as needed)
ENTRYPOINT ["dotnet", "Feliciabot.dll"]
