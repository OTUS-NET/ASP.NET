using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Services.Date.Abstractions;
using PromoCodeFactory.Services.Partners;
using PromoCodeFactory.Services.Partners.Dto;

namespace PromoCodeFactory.UnitTests.Services.Partners.Model.Builder;

public class PartnersServiceTestsModelBuilder
{
    private readonly IFixture _fixture;
    private readonly PartnersServiceTestsModel _model;

    public PartnersServiceTestsModelBuilder()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _model = new PartnersServiceTestsModel();
    }

    public static PartnersServiceTestsModelBuilder Build()
    {
        return new PartnersServiceTestsModelBuilder();
    }

    public PartnersServiceTestsModelBuilder WithPartner(
        bool withActiveLimit = false,
        int numberIssuedPromoCodes = 0,
        bool isActive = true)
    {
        _model.PromoCodeLimitCollectionMock = _fixture.Freeze<Mock<ICollection<PartnerPromoCodeLimit>>>();
        
        _model.Partner = _fixture.Freeze<Partner>(c => c.
            With(p => p.IsActive, isActive).
            With(p => p.NumberIssuedPromoCodes, numberIssuedPromoCodes).
            With(p => p.PartnerLimits, _model.PromoCodeLimitCollectionMock.Object));

        if (withActiveLimit)
        {
            _model.Partner.PartnerLimits = _fixture.CreateMany<PartnerPromoCodeLimit>(1).ToList();
            _model.ActiveLimit = _model.Partner.PartnerLimits.First();
            _model.ActiveLimit.CancelDate = null;
        }

        return this;
    }

    public PartnersServiceTestsModelBuilder WithSetPartnerPromoCodeLimitDto(int limit = 1)
    {
        _model.SetPartnerPromoCodeLimitDto = _fixture.Build<SetPartnerPromoCodeLimitDto>().
            With(r => r.Limit, limit).
            Create();
        return this;
    }

    public PartnersServiceTestsModelBuilder WithDateTimeProviderMock(DateTime dateTime)
    {
        _fixture.Freeze<Mock<IDateTimeProvider>>().Setup(d => d.CurrentDateTime).Returns(dateTime);
        return this;
    }

    public PartnersServiceTestsModel Create()
    {
        _model.PartnersRepositoryMock = _fixture.Freeze<Mock<IRepository<Partner>>>();
        _model.PartnersRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_model.Partner);

        _model.Service = _fixture.Build<PartnersService>().OmitAutoProperties().Create();
        
        return _model;
    }
}