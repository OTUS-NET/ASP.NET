namespace PromoCodeFactory.WebHost.Models
{
    public class UpdateEmployeeRequest
    {
        public string FirstName { get; init; }

        public string LastName { get; init; }

        public string Email { get; init; }

        public int AppliedPromocodesCount { get; set; }
    }
}
