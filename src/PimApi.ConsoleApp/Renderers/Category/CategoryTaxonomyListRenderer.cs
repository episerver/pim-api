using PimApi.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PimApi.ConsoleApp.Renderers.Category
{
    internal class CategoryTaxonomyListRenderer : IApiResponseMessageRenderer
    {
        internal static readonly IApiResponseMessageRenderer Default = new CategoryTaxonomyListRenderer();

        public async Task Render(
            ApiResponseMessage apiResponseMessage,
            IJsonSerializer jsonSerializer,
            Action<string> messageWriter)
        {
            var taxonomies = await apiResponseMessage
                .GetDataAsync<ODataResponseCollection<CategoryTaxonomyDto>>(jsonSerializer);

            RenderCategoryTable(messageWriter, taxonomies.Value);
            taxonomies.WriteTotalCount(messageWriter);
        }

        internal static void RenderCategoryTable(
            Action<string> messageWriter,
            System.Collections.Generic.ICollection<CategoryTaxonomyDto> list)
        {
            var table = new ConsoleTables.ConsoleTable(
                            nameof(CategoryTaxonomyDto.Id),
                            nameof(CategoryTaxonomyDto.Name),
                            nameof(CategoryTaxonomyWebsiteDto.WebsiteId));

            foreach (var entity in list)
            {
                table.AddRow(
                    entity.Id,
                    entity.Name,
                    string.Join(',', entity.Websites.Select(o => o.WebsiteId)));
            };

            messageWriter(table.ToString());
        }
    }
}