using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PromoCodeFactory.Compose;
using PromoCodeFactory.WebHost.Helpers;
using AutoFixture;
using FluentAssertions;
using Moq;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.WebHost.Controllers;
using Xunit;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.UnitTests.Helps;
using AutoFixture.Xunit2;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners.Fixtures
{
    public class Fixture_InMemory : IDisposable
    {
        public IServiceProvider ServiceProvider { get; set; }
        public IServiceCollection ServiceCollection { get; set; }

        /// <summary>
        /// Выполняется перед запуском тестов
        /// </summary>
        public Fixture_InMemory()
        {
            var builder = new ConfigurationBuilder();
            var configuration = builder.Build();
            ServiceCollection = configuration.GetServiceCollection();
            var serviceProvider = GetServiceProvider();
            ServiceProvider = serviceProvider;
        }

        private IServiceProvider GetServiceProvider()
        {
            var serviceProvider = ServiceCollection
                .ConfigureInMemoryContext()
                .BuildServiceProvider();
            serviceProvider.Seed();
            return serviceProvider;
        }

        public void Dispose()
        {
        }
    }
}

