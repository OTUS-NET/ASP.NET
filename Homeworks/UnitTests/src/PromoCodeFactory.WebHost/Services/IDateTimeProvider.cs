using System;

namespace PromoCodeFactory.WebHost.Services;

public interface IDateTimeProvider
{
    DateTime CurrentDateTime { get; }
}

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime CurrentDateTime => DateTime.Now;
}