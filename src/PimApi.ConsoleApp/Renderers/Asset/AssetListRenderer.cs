using PimApi.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PimApi.ConsoleApp.Renderers.Asset
{
    internal class AssetListRenderer : IApiResponseMessageRenderer
    {
        public static readonly IApiResponseMessageRenderer Default = new AssetListRenderer();

        public async Task Render(
            ApiResponseMessage apiResponseMessage,
            IJsonSerializer jsonSerializer,
            Action<string> messageWriter)
        {
            var assets = await apiResponseMessage
                .GetDataAsync<ODataResponseCollection<AssetDto>>(jsonSerializer);

            RenderTable(messageWriter, assets.Value);
            assets.WriteTotalCount(messageWriter);
        }

        internal static void RenderTable(
            Action<string> messageWriter,
            ICollection<AssetDto> assets)
        {
            messageWriter("Assets");
            var table = new ConsoleTables.ConsoleTable(
                nameof(AssetDto.Id),
                nameof(AssetDto.Name),
                nameof(AssetDto.UrlSmall));

            foreach (var entity in assets)
            {
                table.AddRow(
                    entity.Id,
                    entity.Name,
                    entity.UrlSmall);
            }

            messageWriter(table.ToString());
        }
    }
}