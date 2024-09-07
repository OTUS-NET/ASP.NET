namespace PromoCodeFactory.Core.Exceptions
{
    public class RoleIsNotFoundException(string message = "Couldn`t find the Role.")
        : NotFoundException(message)
    {
    }
}
