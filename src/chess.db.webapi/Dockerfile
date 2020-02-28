#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["chess.db.webapi/chess.db.webapi.csproj", "chess.db.webapi/"]
COPY ["chess.games.db.api/chess.games.db.api.csproj", "chess.games.db.api/"]
COPY ["AspNetCore.MVC.RESTful/AspNetCore.MVC.RESTful.csproj", "AspNetCore.MVC.RESTful/"]
COPY ["chess.games.db/chess.games.db.csproj", "chess.games.db/"]
RUN dotnet restore "chess.db.webapi/chess.db.webapi.csproj"
COPY . .
WORKDIR "/src/chess.db.webapi"
RUN dotnet build "chess.db.webapi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "chess.db.webapi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "chess.db.webapi.dll"]