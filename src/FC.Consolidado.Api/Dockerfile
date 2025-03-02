FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/FC.Consolidado.Api/FC.Consolidado.Api.csproj", "src/FC.Consolidado.Api/"]
COPY ["src/FC.ServiceDefaults/FC.ServiceDefaults.csproj", "src/FC.ServiceDefaults/"]
COPY ["src/FC.Core/FC.Core.csproj", "src/FC.Core/"]
COPY ["src/FC.MessageBus/FC.MessageBus.csproj", "src/FC.MessageBus/"]
RUN dotnet restore "src/FC.Consolidado.Api/FC.Consolidado.Api.csproj"
COPY . .
WORKDIR "/src/src/FC.Consolidado.Api"
RUN dotnet build "FC.Consolidado.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FC.Consolidado.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FC.Consolidado.Api.dll"]
