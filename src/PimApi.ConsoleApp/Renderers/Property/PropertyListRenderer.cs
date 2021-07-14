using PimApi.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PimApi.ConsoleApp.Renderers.Property
{
    internal class PropertyListRenderer : IApiResponseMessageRenderer
    {
        public static readonly IApiResponseMessageRenderer Default = new PropertyListRenderer();

        public async Task Render(
            ApiResponseMessage apiResponseMessage,
            IJsonSerializer jsonSerializer,
            Action<string> messageWriter)
        {
            var response = await apiResponseMessage
                .GetDataAsync<ODataResponseCollection<PropertyDto>>(jsonSerializer);

            RenderTable(messageWriter, response.Value);
            response.WriteTotalCount(messageWriter);
        }

        internal static void RenderTable(
            Action<string> messageWriter,
            ICollection<PropertyDto> properties)
        {
            messageWriter("Properties");

            var table = new ConsoleTables.ConsoleTable(
                            nameof(PropertyDto.Id),
                            nameof(PropertyDto.Name),
                            nameof(PropertyDto.DisplayName),
                            nameof(PropertyDto.IsGloballyRequired),
                            nameof(PropertyDto.ControlType));

            foreach (var entity in properties)
            {
                table.AddRow(
                    entity.Id,
                    entity.Name,
                    entity.DisplayName,
                    entity.IsGloballyRequired,
                    entity.ControlType);
            }

            messageWriter(table.ToString());
        }
    }
}