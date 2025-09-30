using System.Net.Http.Json;
using FluentAssertions;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodesFactory.WebHost.Tests;

public class PreferenceControllerTests(WebHostFixture fixture) : IClassFixture<WebHostFixture>
{
    [Fact]
    public async Task Get_Should_Return_Status_200()
    {
        var client = fixture.CreateClient();
        var response = await client.GetAsync("api/v1/Preferences");
        response.Should().Be200Ok();
    }

    [Fact]
    public async Task Get_Should_Return_List_With_One_Customer()
    {
        var client = fixture.CreateClient();
        var response = await client.GetAsync("api/v1/Preferences");
        response.Should().Be200Ok();

        var content = (await response.Content.ReadFromJsonAsync<IEnumerable<CustomerShortResponse>>())?.ToArray();
        content.Should()
            .NotBeNull()
            .And.HaveCount(FakeDataFactory.Preferences.Count());
    }
    
    [Fact]
    public async Task Post_Should_Return_Status_405()
    {
        var client = fixture.CreateClient();
        var response = await client.PostAsJsonAsync("api/v1/Preferences", new { Test = "dummy" });
        response.Should().Be405MethodNotAllowed();
    }
    
    [Fact]
    public async Task Put_Should_Return_Status_404()
    {
        var client = fixture.CreateClient();
        var response = await client.PutAsJsonAsync($"api/v1/Preferences/{Guid.NewGuid():D}", new { Test = "dummy" });
        response.Should().Be404NotFound();
    }
    
    
    [Fact]
    public async Task Delete_Should_Return_Status_404()
    {
        var client = fixture.CreateClient();
        var response = await client.DeleteAsync($"api/v1/Preferences/{Guid.NewGuid():D}");
        response.Should().Be404NotFound();
    }
}