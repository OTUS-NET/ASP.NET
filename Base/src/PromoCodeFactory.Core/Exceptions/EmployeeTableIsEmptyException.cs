namespace PromoCodeFactory.Core.Exceptions
{
    public class EmployeeTableIsEmptyException(string message = "The list of Employee is empty.") 
        : NotFoundException(message)
    {
    }
}
