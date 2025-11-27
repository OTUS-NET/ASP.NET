using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.DataAccess.Repositories;
using Pcf.GivingToCustomer.WebHost.Controllers;
using Pcf.GivingToCustomer.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Pcf.GivingToCustomer.IntegrationTests.Components.WebHost.Controllers
{
    [Collection(EfDatabaseCollection.DbCollection)]
    public class CustomersControllerTests: IClassFixture<EfDatabaseFixture>
    {
        private readonly CustomersController _customersController;
        private readonly EfRepository<Customer> _customerRepository;
        private readonly EfRepository<Preference> _preferenceRepository;
        private readonly Mock<IPreferenceCacheGateway> _preferenceGatewayMock;
        private readonly Mock<ILogger<CustomersController>> _loggerMock;
        public CustomersControllerTests(EfDatabaseFixture efDatabaseFixture)
        {
            _customerRepository = new EfRepository<Customer>(efDatabaseFixture.DbContext);
            _preferenceRepository = new EfRepository<Preference>(efDatabaseFixture.DbContext);

            _preferenceGatewayMock = new Mock<IPreferenceCacheGateway>();
            _preferenceGatewayMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Preference>());
            _preferenceGatewayMock.Setup(s => s.AddPreferencesAsync(new List<Preference>()));

            _loggerMock = new Mock<ILogger<CustomersController>>();
            _customersController = new CustomersController(
                _customerRepository, 
                _preferenceRepository,
                _preferenceGatewayMock.Object,
                _loggerMock.Object
                );
        }
        
        [Fact]
        public async Task CreateCustomerAsync_CanCreateCustomer_ShouldCreateExpectedCustomer()
        {
            //Arrange 
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
            var result = await _customersController.CreateCustomerAsync(request);
            var actionResult = result.Result as CreatedAtActionResult;
            var id = (Guid)actionResult.Value;
            
            //Assert
            var actual = await _customerRepository.GetByIdAsync(id);
            
            actual.Email.Should().Be(request.Email);
            actual.FirstName.Should().Be(request.FirstName);
            actual.LastName.Should().Be(request.LastName);
            actual.Preferences.Should()
                .ContainSingle()
                .And
                .Contain(x => x.PreferenceId == preferenceId);
        }
    }
}