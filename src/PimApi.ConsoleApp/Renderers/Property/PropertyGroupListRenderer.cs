using PimApi.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PimApi.ConsoleApp.Renderers.Property
{
    internal class PropertyGroupListRenderer : IApiResponseMessageRenderer
    {
        public static readonly IApiResponseMessageRenderer Default = new PropertyGroupListRenderer();

        async Task IApiResponseMessageRenderer.Render(
            ApiResponseMessage apiResponseMessage,
            IJsonSerializer jsonSerializer,
            Action<string> messageWriter)
        {
            var response = await apiResponseMessage
                .GetDataAsync<ODataResponse<ICollection<PropertyGroupDto>>>(jsonSerializer);

            var table = new ConsoleTables.ConsoleTable(
                nameof(PropertyGroupDto.Name),
                nameof(PropertyGroupDto.Description),
                nameof(PropertyGroupDto.PropertyCount));

            foreach (var entity in response.Value)
            {
                table.AddRow(entity.Name, entity.Description, entity.PropertyCount());
            }

            messageWriter(table.ToString());
            response.WriteTotalCount(messageWriter);
        }
    }
}