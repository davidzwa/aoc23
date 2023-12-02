FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AOC23/AOC23.csproj", "AOC23/"]
RUN dotnet restore "AOC23/AOC23.csproj"
COPY . .
WORKDIR "/src/AOC23"
RUN dotnet build "AOC23.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AOC23.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AOC23.dll"]
