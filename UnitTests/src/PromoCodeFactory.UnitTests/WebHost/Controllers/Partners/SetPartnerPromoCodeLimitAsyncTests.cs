public class SetPartnerPromoCodeLimitAsyncTests
{
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly PartnersController _partnersController;

    public SetPartnerPromoCodeLimitAsyncTests()
    {
        _partnersRepositoryMock = new Mock<IRepository<Partner>>();
        _partnersController = new PartnersController(_partnersRepositoryMock.Object);
    }

    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_PartnerNotFound_ReturnsNotFound()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId)).ReturnsAsync((Partner)null);

        // Act
        var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, SetPartnerPromoCodeLimitRequestBuilder.CreateValidRequest(100, DateTime.Now.AddDays(30)));

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotActive_ReturnsBadRequest()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var partner = PartnerBuilder.CreateInactivePartner(partnerId);
        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId)).ReturnsAsync(partner);

        // Act
        var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, SetPartnerPromoCodeLimitRequestBuilder.CreateValidRequest(100, DateTime.Now.AddDays(30)));

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("Данный партнер не активен");
    }

    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_ValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var partner = PartnerBuilder.CreateActivePartner(partnerId);
        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId)).ReturnsAsync(partner);

        var request = SetPartnerPromoCodeLimitRequestBuilder.CreateValidRequest(100, DateTime.Now.AddDays(30));

        // Act
        var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtActionResult = result as CreatedAtActionResult;
        createdAtActionResult.ActionName.Should().Be(nameof(PartnersController.GetPartnerLimitAsync));
        createdAtActionResult.RouteValues["id"].Should().Be(partnerId);
        createdAtActionResult.RouteValues["limitId"].Should().NotBeNull();
    }

    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_InvalidLimit_ReturnsBadRequest()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var partner = PartnerBuilder.CreateActivePartner(partnerId);
        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId)).ReturnsAsync(partner);

        var request = SetPartnerPromoCodeLimitRequestBuilder.CreateInvalidLimitRequest(0);

        // Act
        var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("Лимит должен быть больше 0");
    }

    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_ActiveLimitExists_CancelsPreviousLimit()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var partner = PartnerBuilder.CreatePartnerWithActiveLimit(partnerId, 50);
        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId)).ReturnsAsync(partner);

        var request = SetPartnerPromoCodeLimitRequestBuilder.CreateValidRequest(100, DateTime.Now.AddDays(30));

        // Act
        var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        partner.PartnerLimits.First().CancelDate.Should().NotBeNull();
        partner.NumberIssuedPromoCodes.Should().Be(0);
    }
}