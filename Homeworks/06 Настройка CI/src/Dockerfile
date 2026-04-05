FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet build -c $BUILD_CONFIGURATION --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish ./PromoCodeFactory.WebHost/PromoCodeFactory.WebHost.csproj \
    -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-build

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "PromoCodeFactory.WebHost.dll"]
