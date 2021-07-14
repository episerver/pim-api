using FluentAssertions;
using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PimApi.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture(Category = nameof(Extensions))]
    [Parallelizable(ParallelScope.All)]
    public class ApiHttpClientFactoryTests
    {
        [Test]
        public void CreateShouldThrowArgumentExceptionWhenBadUrl()
        {
            var connectionInformation = new ConnectionInformation
            {
                PimUrlBase = null!
            };

            var httpClientFactory = new ApiHttpClientFactory(connectionInformation);

            Action act = () => httpClientFactory.Create();

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void CreateShouldThrowArgumentExceptionWhenNullConnectionInformation()
        {
            var httpClientFactory = new ApiHttpClientFactory(null!);

            Action act = () => httpClientFactory.Create();

            act.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}
