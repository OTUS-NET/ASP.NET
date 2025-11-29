
dotnet ef migrations add Initial --startup-project PromoCodeFactory.WebHost/PromoCodeFactory.WebHost.csproj --project PromoCodeFactory.EntityFramework\PromoCodeFactory.EntityFramework.csproj --context DataContext

dotnet ef database update --startup-project PromoCodeFactory.WebHost/PromoCodeFactory.WebHost.csproj --project PromoCodeFactory.EntityFramework\PromoCodeFactory.EntityFramework.csproj --context DataContext


PAUSE