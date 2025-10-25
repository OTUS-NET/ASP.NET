# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Pcf.GivingToCustomer/Pcf.GivingToCustomer.GrpcHost/Pcf.GivingToCustomer.GrpcHost.csproj", "Pcf.GivingToCustomer/Pcf.GivingToCustomer.GrpcHost/"]
COPY ["Pcf.GivingToCustomer/Pcf.GivingToCustomer.Core/Pcf.GivingToCustomer.Core.csproj", "Pcf.GivingToCustomer/Pcf.GivingToCustomer.Core/"]
COPY ["Pcf.GivingToCustomer/Pcf.GivingToCustomer.DataAccess/Pcf.GivingToCustomer.DataAccess.csproj", "Pcf.GivingToCustomer/Pcf.GivingToCustomer.DataAccess/"]
RUN dotnet restore "./Pcf.GivingToCustomer/Pcf.GivingToCustomer.GrpcHost/Pcf.GivingToCustomer.GrpcHost.csproj"
COPY . .
WORKDIR "/src/Pcf.GivingToCustomer/Pcf.GivingToCustomer.GrpcHost"
RUN dotnet build "./Pcf.GivingToCustomer.GrpcHost.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Pcf.GivingToCustomer.GrpcHost.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pcf.GivingToCustomer.GrpcHost.dll"]