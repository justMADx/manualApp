FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["manualApp/manualApp.csproj", "manualApp/"]
RUN dotnet restore "manualApp/manualApp.csproj"
COPY . .
WORKDIR "/src/manualApp"
RUN dotnet build "manualApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "manualApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "manualApp.dll"]
