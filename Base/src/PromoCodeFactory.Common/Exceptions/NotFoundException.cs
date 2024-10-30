namespace PromoCodeFactory.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException()
        : base("Resource not found")
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }
}