using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Services;
using PromoCodeFactory.WebHost.Services.Date.Abstractions;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners.Model.Builder;

public class PromoCodeControllerTestsModelBuilder
{
    private readonly IFixture _fixture;
    private readonly PromoCodeControllerTestsModel _model;

    public PromoCodeControllerTestsModelBuilder()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _model = new PromoCodeControllerTestsModel();
    }

    public static PromoCodeControllerTestsModelBuilder Build()
    {
        return new PromoCodeControllerTestsModelBuilder();
    }

    public PromoCodeControllerTestsModelBuilder WithPartner(
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

    public PromoCodeControllerTestsModelBuilder WithRequest(int limit = 1)
    {
        _model.Request = _fixture.Build<SetPartnerPromoCodeLimitRequest>().
            With(r => r.Limit, limit).
            Create();
        return this;
    }

    public PromoCodeControllerTestsModelBuilder WithDateTimeProviderMock(DateTime dateTime)
    {
        _fixture.Freeze<Mock<IDateTimeProvider>>().Setup(d => d.CurrentDateTime).Returns(dateTime);
        return this;
    }

    public PromoCodeControllerTestsModel Create()
    {
        _model.PartnersRepositoryMock = _fixture.Freeze<Mock<IRepository<Partner>>>();
        _model.PartnersRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_model.Partner);

        _model.Controller = _fixture.Build<PartnersController>().OmitAutoProperties().Create();
        
        return _model;
    }
}