FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY DocSpot.sln ./
COPY DocSpot.WebAPI/DocSpot.WebAPI.csproj DocSpot.WebAPI/
COPY DocSpot.Core/DocSpot.Core.csproj DocSpot.Core/
COPY DocSpot.Infrastructure/DocSpot.Infrastructure.csproj DocSpot.Infrastructure/

RUN dotnet restore

COPY . .
RUN dotnet publish DocSpot.WebAPI/DocSpot.WebAPI.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
ENTRYPOINT ["dotnet", "DocSpot.WebAPI.dll"]
