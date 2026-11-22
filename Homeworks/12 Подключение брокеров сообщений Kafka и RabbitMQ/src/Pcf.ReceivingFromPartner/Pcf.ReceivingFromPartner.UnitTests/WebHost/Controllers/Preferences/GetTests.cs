using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pcf.Common.Core.Abstractions.Repositories;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.WebHost.Controllers;
using Pcf.ReceivingFromPartner.WebHost.Models.Preferences;

namespace Pcf.ReceivingFromPartner.UnitTests.WebHost.Controllers.Preferences;

public class GetTests
{
    private readonly Mock<IRepository<Preference>> _preferencesRepositoryMock;
    private readonly PreferencesController _sut;

    public GetTests()
    {
        _preferencesRepositoryMock = new Mock<IRepository<Preference>>();
        _sut = new PreferencesController(_preferencesRepositoryMock.Object);
    }

    [Fact]
    public async Task Get_ReturnsPreferences()
    {
        var preference = new Preference
        {
            Id = Guid.NewGuid(),
            Name = "Theater"
        };

        _preferencesRepositoryMock
            .Setup(r => r.GetAll(false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Preference> { preference });

        var result = await _sut.Get(CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = ok.Value.Should().BeAssignableTo<IEnumerable<PreferenceShortResponse>>().Subject.Single();
        response.Id.Should().Be(preference.Id);
        response.Name.Should().Be("Theater");
    }

    [Fact]
    public async Task Get_WhenRepositoryIsEmpty_ReturnsEmptyList()
    {
        _preferencesRepositoryMock
            .Setup(r => r.GetAll(false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Preference>());

        var result = await _sut.Get(CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeAssignableTo<IEnumerable<PreferenceShortResponse>>().Which.Should().BeEmpty();
    }
}
