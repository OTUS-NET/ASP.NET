namespace PromoCodeFactory.Services.Date.Abstractions;

public interface IDateTimeProvider
{
    DateTime CurrentDateTime { get; }
}