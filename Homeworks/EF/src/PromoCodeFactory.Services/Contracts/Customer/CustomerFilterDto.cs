namespace PromoCodeFactory.Services.Contracts.Customer;

public class CustomerFilterDto
{
    public List<string>? Preferences { get; set; }
    public List<string>? PromoCodes { get; set; }
}