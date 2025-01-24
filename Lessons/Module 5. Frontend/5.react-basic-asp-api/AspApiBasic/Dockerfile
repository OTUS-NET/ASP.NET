# Сборка образа для ASP.NET приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

# Копирование файлов для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AspApiBasic/AspApiBasic.csproj", "AspApiBasic/"]
RUN dotnet restore "AspApiBasic/AspApiBasic.csproj"
COPY . .
WORKDIR "/src/AspApiBasic"
RUN dotnet build "AspApiBasic.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Сборка приложения
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AspApiBasic.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Копирование файлов в контейнер для запуска приложения
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Установка произвольного порта для ASP.NET приложения
ENV ASPNETCORE_URLS=http://+:5687 

# Открытие порта в контейнере
EXPOSE 5687

# Запуск приложения
ENTRYPOINT ["dotnet", "AspApiBasic.dll"]