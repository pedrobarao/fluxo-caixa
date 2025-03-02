FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/FC.Lancamentos.Api/FC.Lancamentos.Api.csproj", "src/FC.Lancamentos.Api/"]
COPY ["src/FC.ServiceDefaults/FC.ServiceDefaults.csproj", "src/FC.ServiceDefaults/"]
COPY ["src/FC.Core/FC.Core.csproj", "src/FC.Core/"]
COPY ["src/FC.Lancamentos.Application/FC.Lancamentos.Application.csproj", "src/FC.Lancamentos.Application/"]
COPY ["src/FC.Lancamentos.Domain/FC.Lancamentos.Domain.csproj", "src/FC.Lancamentos.Domain/"]
COPY ["src/FC.MessageBus/FC.MessageBus.csproj", "src/FC.MessageBus/"]
RUN dotnet restore "src/FC.Lancamentos.Api/FC.Lancamentos.Api.csproj"
COPY . .
WORKDIR "/src/src/FC.Lancamentos.Api"
RUN dotnet build "FC.Lancamentos.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FC.Lancamentos.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FC.Lancamentos.Api.dll"]
