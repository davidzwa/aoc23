FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["q4/q4.csproj", "q4/"]
RUN dotnet restore "q4/q4.csproj"
COPY . .
WORKDIR "/src/q4"
RUN dotnet build "q4.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "q4.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "q4.dll"]
