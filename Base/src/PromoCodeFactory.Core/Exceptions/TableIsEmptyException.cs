namespace PromoCodeFactory.Core.Exceptions
{
    public abstract class TableIsEmptyException(string message) 
        : NotFoundException(message)
    {
    }
}
