using System.Net.Http.Json;
using FluentAssertions;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodesFactory.WebHost.Tests;

public class PromocodesControllerTests(WebHostFixture fixture) : IClassFixture<WebHostFixture>
{
    [Fact]
    public async Task Get_Should_Return_Status_200()
    {
        var client = fixture.GetClient();
        var response = await client.GetAsync("api/v1/Promocodes");
        response.Should().Be200Ok();
    }

    [Fact]
    public async Task Get_Should_Return_Empty_List()
    {
        var client = fixture.GetClient();
        var response = await client.GetAsync("api/v1/Promocodes");
        response.Should().Be200Ok();

        var content = (await response.Content.ReadFromJsonAsync<IEnumerable<PromoCodeShortResponse>>())?.ToArray();
        content.Should()
            .NotBeNull()
            .And.BeEmpty();
    }
    
    [Fact]
    public async Task Post_Should_Return_Status_200()
    {
        var client = fixture.GetClient(true);
        var response = await client.PostAsJsonAsync("api/v1/Promocodes", new GivePromoCodeRequest
        {
            PartnerName = FakeDataFactory.Employees.First().FullName,
            ServiceInfo = "SvcInfo New",
            Preference = "Театр",
            PromoCode = "CODE_TEST"
        });
        response.Should().Be200Ok();
        
        //TODO: add extended testing
    }
}