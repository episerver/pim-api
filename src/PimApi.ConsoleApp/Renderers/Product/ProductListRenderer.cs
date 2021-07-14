using PimApi.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PimApi.ConsoleApp.Renderers.Product
{
    internal class ProductListRenderer : IApiResponseMessageRenderer
    {
        public static readonly IApiResponseMessageRenderer Default =
            new ProductListRenderer();

        public async Task Render(
            ApiResponseMessage apiResponseMessage,
            IJsonSerializer jsonSerializer,
            Action<string> messageWriter)
        {
            var response = await apiResponseMessage
                .GetDataAsync<ODataResponseCollection<ProductDto>>(jsonSerializer);

            RenderProductsTable(messageWriter, response.Value);
            response.WriteTotalCount(messageWriter);
        }

        internal static void RenderProductsTable(Action<string> messageWriter, ICollection<ProductDto> products)
        {
            messageWriter("Products");

            var table = new ConsoleTables.ConsoleTable(
                            nameof(ProductDto.Id),
                            nameof(ProductDto.ProductNumber),
                            nameof(ProductDto.ProductTitle),
                            nameof(ProductDto.Status),
                            nameof(ProductDto.LastPublishedOn));

            foreach (var entity in products)
            {
                table.AddRow(
                    entity.Id,
                    entity.ProductNumber,
                    entity.ProductTitle,
                    entity.Status,
                    entity.LastPublishedOn.DisplayValue());
            }

            messageWriter(table.ToString());
        }
    }
}