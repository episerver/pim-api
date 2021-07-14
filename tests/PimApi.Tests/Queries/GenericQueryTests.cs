using NUnit.Framework;
using PimApi.ConsoleApp;
using PimApi.ConsoleApp.Queries.Miscellaneous;
using PimApi.ConsoleApp.Renderers.Miscellaneous;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using static PimApi.Tests.TestSetup;

namespace PimApi.Tests.Queries
{
    [ExcludeFromCodeCoverage]
    [TestFixture(Category = nameof(GenericQuery))]
    [Parallelizable(ParallelScope.All)]
    public class GenericQueryTests
    {
        [TestCase(SystemTextJsonSerializer)]
        [TestCase(NewtonsoftJsonSerializer)]
        public async Task ShouldResolveAndDeserializeDefaultQuery(string serializerKey)
        {
            var jsonSerializer = serializerKey.GetJsonSerializer();
            var query = new GenericQuery();

            using var result = query.Execute(ApiClient);

            await query.ShouldRenderMessage(result, jsonSerializer);
        }

        [TestCase(SystemTextJsonSerializer)]
        [TestCase(NewtonsoftJsonSerializer)]
        public async Task ShouldResolveAndDeserializeCustomQueryWithInvalidRendererChoice(string serializerKey)
        {
            var jsonSerializer = serializerKey.GetJsonSerializer();
            var renderer = new UserChoiceRenderer(new List<IApiResponseMessageRenderer>
            {
                new EntityListRenderer(),
                new TemplateDetailRenderer(),
                new TemplateListRenderer()
            })
            {
                ChosenRenderer = 1000
            };

            var query = new GenericQuery(renderer)
            {
                QueryText = "products?$top=1",
            };

            using var result = query.Execute(ApiClient);

            await query.ShouldRenderMessage(result, jsonSerializer);
        }
    }
}