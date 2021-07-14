using PimApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PimApi.ConsoleApp.Renderers.Miscellaneous
{
    internal class WebsiteListRenderer : IApiResponseMessageRenderer
    {
        internal static readonly IApiResponseMessageRenderer Default = new WebsiteListRenderer();

        public async Task Render(
            ApiResponseMessage apiResponseMessage,
            IJsonSerializer jsonSerializer,
            Action<string> messageWriter)
        {
            var response = await apiResponseMessage
                .GetDataAsync<ODataResponseCollection<WebsiteDto>>(jsonSerializer);

            RenderTable(messageWriter, response.Value);
            response.WriteTotalCount(messageWriter);
        }

        internal static void RenderTable(
            Action<string> messageWriter,
            ICollection<WebsiteDto> websites)
        {
            messageWriter("Websites");

            var table = new ConsoleTables.ConsoleTable(
                nameof(WebsiteDto.Id),
                nameof(WebsiteDto.Name),
                nameof(WebsiteDto.IscWebsiteId),
                nameof(WebsiteDto.CategoryTaxonomies));

            foreach (var entity in websites)
            {
                table.AddRow(
                    entity.Id,
                    entity.Name,
                    entity.IscWebsiteId,
                    string.Join('|',
                        entity.CategoryTaxonomies.Select(o => o.CategoryTaxonomyId)));
            }

            messageWriter(table.ToString());
        }
    }
}