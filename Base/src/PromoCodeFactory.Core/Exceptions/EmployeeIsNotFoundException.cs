namespace PromoCodeFactory.Core.Exceptions
{
    public class EmployeeIsNotFoundException(string message = "Couldn`t find the Employee.")
        : NotFoundException(message)
    {
    }
}
