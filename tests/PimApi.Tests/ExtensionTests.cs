using PimApi.ConsoleApp.Queries.Product;
using PimApi.Extensions;
using System.Net.Http;

namespace PimApi.Tests;

[ExcludeFromCodeCoverage]
[TestFixture(Category = nameof(Extensions))]
[Parallelizable(ParallelScope.All)]
public class ExtensionTests
{
    private class TestQuery : IQuery
    {
        public ApiResponseMessage Execute(HttpClient pimApiClient)
        {
            throw new NotImplementedException();
        }
    }

    [Test]
    public void ShouldGetQueryDescriptorFromqueryWithNoAttribute()
    {
        var query = new TestQuery();

        var descriptor = query.GetQueryDescriptor();

        descriptor.Group.Should().Be("Other");
        descriptor.QueryNumber.Should().Be(0);
        descriptor.Description.Should().Be("n/a");
        descriptor.DisplayName.Should().Be(nameof(TestQuery));
        descriptor.IsSet().Should().BeTrue();
        descriptor.Query.Should().BeSameAs(query);
    }

    [Test]
    public void ShouldGetQueryDescriptorFromquery()
    {
        var query = new GetByProductId();

        var descriptor = query.GetQueryDescriptor();

        descriptor.Group.Should().Be(nameof(ConsoleApp.Queries.Product));
        descriptor.QueryNumber.Should().Be(5);
        descriptor.Description.Should().NotBeNullOrWhiteSpace();
        descriptor.DisplayName.Should().Be(nameof(GetByProductId));
        descriptor.IsSet().Should().BeTrue();
        descriptor.Query.Should().BeSameAs(query);
    }

    [TestCase(null, false)]
    [TestCase("no", false)]
    [TestCase("Yes", true)]
    [TestCase("yes", true)]
    [TestCase("y", true)]
    [TestCase("n", false)]
    [Test]
    public void ShouldBeExpectedForStringInput(string? input, bool expected)
    {
        input.IsYes().Should().Be(expected);
    }

    [Test]
    public void ShouldBeFallbackForNullGuidValue()
    {
        var fallback = Guid.NewGuid();
        string? guidString = null;

        guidString.GetValueOrFallback(fallback)
            .Should()
            .Be(fallback);
    }

    [Test]
    public void ShouldBeNullFallbackForNullGuidValue()
    {
        string? guidString = null;

        guidString.GetValueOrFallback<Guid?>(null)
            .Should()
            .Be(null);
    }

    [Test]
    public void ShouldBeNullFallbackForBadGuidValue()
    {
        var fallback = Guid.NewGuid();
        string? guidString = nameof(ShouldBeNullFallbackForBadGuidValue);

        guidString.GetValueOrFallback<Guid?>(fallback)
            .Should()
            .Be(fallback);
    }

    [Test]
    public void ShouldBeGuidForGuidStringValue()
    {
        var guid = Guid.NewGuid();
        string? guidString = guid.ToString();

        guidString.GetValueOrFallback<Guid?>(null)
            .Should()
            .Be(guid);
    }

    [Test]
    public void ShouldBeFallbackForBadNumberStringValue()
    {
        string? numberString = "1234a";

        numberString.GetValueOrFallback(5)
            .Should()
            .Be(5);
    }

    [Test]
    public void ShouldBeInputStringForValidString()
    {
        const string fallback = nameof(fallback);
        const string inputString = nameof(inputString);

        inputString.GetValueOrFallback(fallback)
            .Should()
            .Be(inputString);
    }

    [Test]
    public void ShouldBeInputIntForValidString()
    {
        const int fallback = 999;
        const string inputString = "100";

        inputString.GetValueOrFallback(fallback)
            .Should()
            .Be(100);
    }

    [TestCase("true", false, true)]
    [TestCase("True", false, true)]
    [TestCase("false", true, false)]
    [TestCase("False", true, false)]
    [Test]
    public void ShouldBeInputBoolForValidString(
        string validBoolString,
        bool fallback,
        bool expected)
    {
        validBoolString.GetValueOrFallback(fallback)
            .Should()
            .Be(expected);
    }
}
