FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["q2/q2.csproj", "q2/"]
RUN dotnet restore "q2/q2.csproj"
COPY . .
WORKDIR "/src/q2"
RUN dotnet build "q2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "q2.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "q2.dll"]
