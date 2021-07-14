using PimApi.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PimApi.ConsoleApp.Renderers.Miscellaneous
{
    internal class TemplateListRenderer : IApiResponseMessageRenderer
    {
        internal static readonly IApiResponseMessageRenderer Default = new TemplateListRenderer();

        public async Task Render(
            ApiResponseMessage apiResponseMessage,
            IJsonSerializer jsonSerializer,
            Action<string> messageWriter)
        {
            var response = await apiResponseMessage
                .GetDataAsync<ODataResponseCollection<TemplateDto>>(jsonSerializer);

            RenderTable(messageWriter, response.Value);
            response.WriteTotalCount(messageWriter);
        }

        internal static void RenderTable(
            Action<string> messageWriter,
            ICollection<TemplateDto> templates)
        {
            messageWriter("Templates");

            var table = new ConsoleTables.ConsoleTable(
                            nameof(TemplateDto.Id),
                            nameof(TemplateDto.Name),
                            nameof(TemplateDto.IsStarterTemplate),
                            nameof(TemplateDto.ParentTemplate),
                            nameof(TemplateDto.ChildrenTemplatesCount),
                            nameof(TemplateDto.TemplatePropertyGroupCount));

            foreach (var entity in templates)
            {
                table.AddRow(
                    entity.Id,
                    entity.Name,
                    entity.IsStarterTemplate,
                    entity.ParentTemplate?.Name,
                    entity.ChildrenTemplatesCount(),
                    entity.TemplatePropertyGroupCount());
            }

            messageWriter(table.ToString());
        }
    }
}