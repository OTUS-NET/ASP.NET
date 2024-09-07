using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.IntegrationTests.Fakes;
using Pcf.GivingToCustomer.WebHost;
using Pcf.GivingToCustomer.WebHost.Models;
using Xunit;

namespace Pcf.GivingToCustomer.IntegrationTests.Api.WebHost.Controllers
{

    public class CustomersControllerTests
        //: IClassFixture<WebApplicationFactory<Startup>> // Postgres
        : IClassFixture<TestWebApplicationFactory<Startup>> // SqlLite
    {
        private readonly WebApplicationFactory<Startup> _factory;
        
        public CustomersControllerTests(
            //WebApplicationFactory<Startup> factory  // Postgres
            TestWebApplicationFactory<Startup> factory // SqlLite
            )
        {
            _factory = factory;
        }
        
        [Fact]
        public async Task CreateCustomerAsync_CanCreateCustomer_ShouldCreateExpectedCustomer()
        {
            //Arrange 
            var client = _factory.CreateClient();
            
            var preferenceId = Guid.Parse("ef7f299f-92d7-459f-896e-078ed53ef99c");
            var request = new CreateOrEditCustomerRequest()
            {
                Email = "some@mail.ru",
                FirstName = "Иван",
                LastName = "Петров",
                PreferenceIds = new List<Guid>()
                {
                    preferenceId
                }
            };

            //Act
            var response = await client.PostAsJsonAsync("/api/v1/customers", request);
         
            //Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            //Теперь получаем объект, который должен было создан, если REST правильно написан, то в Location будет
            //готовый URL для получения нового объекта
            var actualContent = await client.GetStringAsync(response.Headers.Location);
            var actual = JsonConvert.DeserializeObject<CustomerResponse>(actualContent);

            actual.Email.Should().Be(request.Email);
            actual.FirstName.Should().Be(request.FirstName);
            actual.LastName.Should().Be(request.LastName);
            actual.Preferences.Should()
                .ContainSingle()
                .And
                .Contain(x => x.Id == preferenceId);
        }
        
        [Fact]
        public async Task GetCustomerAsync_CustomerExisted_ShouldReturnExpectedCustomer()
        {
            //Arrange 
            //var client = _factory.CreateClient();
            
            //Переопределяем как угодно
            //_factory.WithWebHostBuilder((builder) => { }).CreateClient();
            
            //Переопределяем реальные зависимости заглушками
            var client = _factory.WithWebHostBuilder((builder) =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<INotificationGateway, FakeNotificationGateway>();
                });
            }).CreateClient();

            var expected = new CustomerResponse()
            {
                Id = Guid.Parse("a6c8c6b1-4349-45b0-ab31-244740aaf0f0"),
                Email = "ivan_sergeev@mail.ru",
                FirstName = "Иван",
                LastName = "Петров",
                Preferences = new List<PreferenceResponse>()
                {
                    new PreferenceResponse()
                    {
                        Id = Guid.Parse("ef7f299f-92d7-459f-896e-078ed53ef99c"),
                        Name = "Театр",
                    },
                    new PreferenceResponse()
                    {
                        Id = Guid.Parse("76324c47-68d2-472d-abb8-33cfa8cc0c84"),
                        Name = "Дети",                    
                    }
                }
            };

            //Act
            var response = await client.GetAsync($"/api/v1/customers/{expected.Id}");
         
            //Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var actual = JsonConvert.DeserializeObject<CustomerResponse>(
                await response.Content.ReadAsStringAsync());

            actual.Should().BeEquivalentTo(expected);
        }
    }
}