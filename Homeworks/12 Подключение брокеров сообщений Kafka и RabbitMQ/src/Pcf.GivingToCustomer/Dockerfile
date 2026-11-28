FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet build -c $BUILD_CONFIGURATION --no-restore

FROM build AS test
ARG BUILD_CONFIGURATION=Release
RUN dotnet test --project ./Pcf.GivingToCustomer/Pcf.GivingToCustomer.UnitTests/Pcf.GivingToCustomer.UnitTests.csproj \
    -c $BUILD_CONFIGURATION --no-build \
    --results-directory /test-results \
    -- --report-xunit-trx --report-xunit-trx-filename test-results.trx

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish ./Pcf.GivingToCustomer/Pcf.GivingToCustomer.WebHost/Pcf.GivingToCustomer.WebHost.csproj \
    -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-build

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Pcf.GivingToCustomer.WebHost.dll"]
