FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["q3ConsoleApp1/q3ConsoleApp1.csproj", "q3ConsoleApp1/"]
RUN dotnet restore "q3ConsoleApp1/q3ConsoleApp1.csproj"
COPY . .
WORKDIR "/src/q3ConsoleApp1"
RUN dotnet build "q3ConsoleApp1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "q3ConsoleApp1.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "q3ConsoleApp1.dll"]
