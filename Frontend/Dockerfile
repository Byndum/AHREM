# Base image for running ASP.NET Core apps
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build and publish stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/

# Copy the project file and restore dependencies
COPY ["./Frontend/Frontend.csproj", "Frontend/"]
RUN dotnet restore "./Frontend/Frontend.csproj"

# Copy the full source code and build
COPY . .
WORKDIR "/src/Frontend/"
RUN dotnet publish "Frontend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Frontend.dll"]
