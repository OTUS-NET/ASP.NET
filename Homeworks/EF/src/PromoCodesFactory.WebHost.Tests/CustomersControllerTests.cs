using System.Net.Http.Json;
using FluentAssertions;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodesFactory.WebHost.Tests;

public class CustomerControllerTests(WebHostFixture fixture) : IClassFixture<WebHostFixture>
{
    
    
    [Fact]
    public async Task Get_Should_Return_Status_200()
    {
        var client = fixture.GetClient();
        var response = await client.GetAsync("api/v1/Customers");
        response.Should().Be200Ok();
    }

    [Fact]
    public async Task Get_Should_Return_List_With_One_Customer()
    {
        var client = fixture.GetClient();
        var response = await client.GetAsync("api/v1/Customers");
        response.Should().Be200Ok();

        var content = (await response.Content.ReadFromJsonAsync<IEnumerable<CustomerShortResponse>>())?.ToArray();
        content.Should()
            .NotBeNull()
            .And.HaveCount(1)
            .And.AllSatisfy(x =>
                {
                    x.Id.Should().Be(FakeDataFactory.Customers.First().Id);
                    x.Email.Should().Be(FakeDataFactory.Customers.First().Email);
                    x.FirstName.Should().Be(FakeDataFactory.Customers.First().FirstName);
                    x.LastName.Should().Be(FakeDataFactory.Customers.First().LastName);
                }
            );
    }
    
    [Fact]
    public async Task Post_Should_Return_Status_201_On_Valid_Request()
    {
        var fakeCustomer = FakeDataFactory.Customers.First();
        var fakePreference = FakeDataFactory.Preferences.First();
        var client = fixture.GetClient(true);
        var response = await client.PostAsJsonAsync("api/v1/Customers", 
            new CreateOrEditCustomerRequest
            {
                FirstName = fakeCustomer.FirstName,
                LastName = fakeCustomer.LastName,
                Email = fakeCustomer.Email,
                PreferenceIds = [fakePreference.Id]
            });
        response.Should().Be201Created();

        var content = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        content.Should()
            .NotBeNull()
            .And.Satisfy<CustomerResponse>(x =>
                {
                    x.Email.Should().Be(fakeCustomer.Email);
                    x.FirstName.Should().Be(fakeCustomer.FirstName);
                    x.LastName.Should().Be(fakeCustomer.LastName);
                    x.Preferences.Contains(fakePreference.Id).Should().BeTrue();
                }
            );
    }
    
    [Fact]
    public async Task Post_Should_Return_Status_400_On_Preference_Missing()
    {
        var fakeCustomer = FakeDataFactory.Customers.First();
        var client = fixture.GetClient(true);
        var response = await client.PostAsJsonAsync("api/v1/Customers", 
            new CreateOrEditCustomerRequest
            {
                FirstName = fakeCustomer.FirstName,
                LastName = fakeCustomer.LastName,
                Email = fakeCustomer.Email,
                PreferenceIds = [Guid.NewGuid()]
            });
        response.Should().Be400BadRequest();
    }
    
    [Fact]
    public async Task Put_Should_Return_Status_200_On_Valid_Request()
    {
        var fakeCustomer = FakeDataFactory.Customers.First();
        var fakePreference1 = FakeDataFactory.Preferences.First();
        var fakePreference2 = FakeDataFactory.Preferences.First();
        var client = fixture.GetClient(true);
        var response = await client.PutAsJsonAsync($"api/v1/Customers/{fakeCustomer.Id:D}", 
            new CreateOrEditCustomerRequest
            {
                FirstName = fakeCustomer.FirstName,
                LastName = fakeCustomer.LastName,
                Email = fakeCustomer.Email,
                PreferenceIds = [fakePreference1.Id,  fakePreference2.Id]
            });
        response.Should().Be200Ok();

        var content = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        content.Should()
            .NotBeNull()
            .And.Satisfy<CustomerResponse>(x =>
                {
                    x.Email.Should().Be(fakeCustomer.Email);
                    x.FirstName.Should().Be(fakeCustomer.FirstName);
                    x.LastName.Should().Be(fakeCustomer.LastName);
                    x.Preferences.Contains(fakePreference1.Id).Should().BeTrue();
                    x.Preferences.Contains(fakePreference2.Id).Should().BeTrue();
                }
            );
    }
    
    [Fact]
    public async Task Put_Should_Return_Status_400_If_Preference_Not_Exists()
    {
        var fakeCustomer = FakeDataFactory.Customers.First();
        var client = fixture.GetClient(true);
        var response = await client.PutAsJsonAsync($"api/v1/Customers/{fakeCustomer.Id:D}", 
            new CreateOrEditCustomerRequest
            {
                FirstName = fakeCustomer.FirstName,
                LastName = fakeCustomer.LastName,
                Email = fakeCustomer.Email,
                PreferenceIds = [Guid.NewGuid()]
            });
        response.Should().Be400BadRequest();
    }
    
    [Fact]
    public async Task Put_Should_Return_Status_404_If_Customer_Not_Exists()
    {
        var fakeCustomer = FakeDataFactory.Customers.First();
        var client = fixture.GetClient(true);
        var response = await client.PutAsJsonAsync($"api/v1/Customers/{Guid.NewGuid():D}", 
            new CreateOrEditCustomerRequest
            {
                FirstName = fakeCustomer.FirstName,
                LastName = fakeCustomer.LastName,
                Email = fakeCustomer.Email,
                PreferenceIds = [Guid.NewGuid()]
            });
        response.Should().Be404NotFound();
    }
}