using PromoCodeFactory.WebHost.Services.Date.Abstractions;

namespace PromoCodeFactory.WebHost.Services.Date;

public class DateTimeProvider : IDateTimeProvider
{
    public System.DateTime CurrentDateTime => System.DateTime.Now;
}