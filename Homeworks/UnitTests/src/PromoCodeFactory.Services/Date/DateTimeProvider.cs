using PromoCodeFactory.Services.Date.Abstractions;

namespace PromoCodeFactory.Services.Date;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime CurrentDateTime => DateTime.Now;
}