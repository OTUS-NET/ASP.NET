using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace PromoCodeFactory.UnitTests.Infrastructure
{
    // Обертка для конфигурирования экземпляра 'Fixture',
    // который используется при создании объектов, обявленных в качестве аргументов метода
    public sealed class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute()
            : base(() => new Fixture().Customize(new AutoMoqCustomization()))
        {
        }
    }
}