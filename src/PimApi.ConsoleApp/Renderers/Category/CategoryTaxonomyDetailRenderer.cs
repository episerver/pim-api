using PimApi.ConsoleApp.Renderers.Miscellaneous;

namespace PimApi.ConsoleApp.Renderers.Category;

internal class CategoryTaxonomyDetailRenderer : IApiResponseMessageRenderer
{
    public static readonly IApiResponseMessageRenderer Default =
        new CategoryTaxonomyDetailRenderer();

    public async Task Render(
        ApiResponseMessage apiResponseMessage,
        IJsonSerializer jsonSerializer,
        Action<string> messageWriter
    )
    {
        var entity = await apiResponseMessage.GetDataAsync<CategoryTaxonomyDto>(jsonSerializer);

        entity.WriteBaseEntityInfo(messageWriter);

        if (entity.Websites.Count == 0)
        {
            return;
        }

        WebsiteListRenderer.RenderTable(
            messageWriter,
            entity.Websites.Select(o => o.Website).OfType<WebsiteDto>().ToList()
        );
    }
}
