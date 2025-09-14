using Microsoft.AspNetCore.Mvc.Testing;

namespace PromoCodeFactory.WebHost.Tests;

public class WebHostFixture() : WebApplicationFactory<Program>
{
    public string ApiRoot = "/api/v1/";

}