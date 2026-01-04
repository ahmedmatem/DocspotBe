# build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY DocSpot.WebAPI/DocSpot.WebAPI.csproj DocSpot.WebAPI/
COPY DocSpot.Core/DocSpot.Core.csproj DocSpot.Core/
COPY DocSpot.Infrastructure/DocSpot.Infrastructure.csproj DocSpot.Infrastructure/

RUN dotnet restore DocSpot.WebAPI/DocSpot.WebAPI.csproj

COPY . .
RUN dotnet publish DocSpot.WebAPI/DocSpot.WebAPI.csproj -c Release -o /app/publish /p:UseAppHost=false

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DocSpot.WebAPI.dll"]

