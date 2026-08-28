# Собираем приложение внутри SDK-образа, чтобы не требовать .NET SDK на хосте.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore PromoCodeFactory.slnx
RUN dotnet build PromoCodeFactory.WebHost/PromoCodeFactory.WebHost.csproj -c Release -o /app/build --no-restore

# Запускаем уже собранное приложение в более легком образом в рантайме.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Контейнер слушает HTTP на порту 8080.
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "PromoCodeFactory.WebHost.dll"]
