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
    public async Task Post_Should_Add_Promocode_To_Customer()
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
        
        var customerResponse = await client.GetAsync($"api/v1/Customers/{FakeDataFactory.Customers.First().Id}");
        customerResponse.Should().Be200Ok();
        
        var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerResponse>();

        customer.Should()
            .NotBeNull()
            .And.Satisfy<CustomerResponse>(x => x.Preferences.Should().Contain(FakeDataFactory.Preferences.First().Id));


    }
    
    [Fact]
    public async Task Post_Should_Not_Add_Promocode_To_Customer()
    {
        var client = fixture.GetClient(true);
        var response = await client.PostAsJsonAsync("api/v1/Promocodes", new GivePromoCodeRequest
        {
            PartnerName = FakeDataFactory.Employees.First().FullName,
            ServiceInfo = "SvcInfo New",
            Preference = "Дети",
            PromoCode = "CODE_TEST"
        });
        response.Should().Be200Ok();
        
        var customerResponse = await client.GetAsync($"api/v1/Customers/{FakeDataFactory.Customers.First().Id}");
        customerResponse.Should().Be200Ok();
        
        var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerResponse>();

        customer.Should()
            .NotBeNull()
            .And.Satisfy<CustomerResponse>(x => x.Preferences.Should().NotContain(FakeDataFactory.Preferences.Last().Id));


    }
    
    [Fact]
    public async Task Post_Should_Add_Customer_Promocode()
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
        
        var customerResponse = await client.GetAsync($"api/v1/Customers/{FakeDataFactory.Customers.First().Id}");
        customerResponse.Should().Be200Ok();
        
        var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerResponse>();

        customer.Should()
            .NotBeNull()
            .And.Satisfy<CustomerResponse>(x => x.Preferences.Should().Contain(FakeDataFactory.Preferences.First().Id));


    }
}