FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/ServiceMatch.API/ServiceMatch.API.csproj", "src/ServiceMatch.API/"]
COPY ["src/ServiceMatch.Application/ServiceMatch.Application.csproj", "src/ServiceMatch.Application/"]
COPY ["src/ServiceMatch.Domain/ServiceMatch.Domain.csproj", "src/ServiceMatch.Domain/"]
COPY ["src/ServiceMatch.Infrastructure/ServiceMatch.Infrastructure.csproj", "src/ServiceMatch.Infrastructure/"]

RUN dotnet restore "src/ServiceMatch.API/ServiceMatch.API.csproj"

COPY . .
RUN dotnet build "src/ServiceMatch.API/ServiceMatch.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/ServiceMatch.API/ServiceMatch.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServiceMatch.API.dll"]
