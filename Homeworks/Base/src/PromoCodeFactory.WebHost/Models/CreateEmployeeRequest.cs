namespace PromoCodeFactory.WebHost.Models
{
    public class CreateEmployeeRequest
    {
        public required string FirstName { get; init; }

        public required string LastName { get; init; }

        public required string Email { get; init; }
    }
}
