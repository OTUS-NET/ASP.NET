using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.UnitTests.TestFixtures;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using System.Threading.Tasks;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners;

public class SetPartnerPromoCodeLimitAsyncInMemoryTests : IClassFixture<FixtureWithInMemoryDatabase>
{
    private readonly IRepository<Partner> partnersRepository;
    private readonly PartnersController partnersController;

    public SetPartnerPromoCodeLimitAsyncInMemoryTests(FixtureWithInMemoryDatabase fixture)
    {
        partnersRepository = fixture.ServiceProvider.GetRequiredService<IRepository<Partner>>();
        partnersController = new PartnersController(partnersRepository);
    }

    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_PositivePath_NewLimitSavedInDb()
    {
        // Arrange
        var partner = new PartnerBuilder().WithActiveState(true)
                                          .WithActiveLimit()
                                          .Build();
        var partnerLimitCount = partner.PartnerLimits.Count;
        await partnersRepository.AddAsync(partner);
        var request = new SetPartnerPromoCodeLimitRequest()
        {
            Limit = 50
        };

        // Act
        await partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

        // Assert
        var partnerFromDb = await partnersRepository.GetByIdAsync(partner.Id);
        var addedLimitCount = partnerFromDb.PartnerLimits.Count - partnerLimitCount;
        addedLimitCount.Should().Be(1);
    }
}