namespace PromoCodeFactory.WebHost.Services.Date.Abstractions;

public interface IDateTimeProvider
{
    System.DateTime CurrentDateTime { get; }
}