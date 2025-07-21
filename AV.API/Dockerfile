
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY AV.API/AV.API.csproj AV.API/
COPY Engine/AV.Engine.Core/AV.Engine.Core.csproj   Engine/AV.Engine.Core/
COPY Engine/AV.Engine.Persistence/AV.Engine.Persistence.csproj  Engine/AV.Engine.Persistence/

RUN dotnet restore AV.API/AV.API.csproj

COPY . .
RUN dotnet publish AV.API/AV.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AV.API.dll"]