namespace PimApi.ConsoleApp.Renderers.Category;

internal class CategoryTreeListRenderer : IApiResponseMessageRenderer
{
    internal static readonly IApiResponseMessageRenderer Default = new CategoryTreeListRenderer();

    public async Task Render(
        ApiResponseMessage apiResponseMessage,
        IJsonSerializer jsonSerializer,
        Action<string> messageWriter
    )
    {
        var categoryTrees = await apiResponseMessage.GetDataAsync<
            ODataResponseCollection<CategoryTreeDto>
        >(jsonSerializer);

        RenderTable(messageWriter, categoryTrees.Value);
        categoryTrees.WriteTotalCount(messageWriter);
    }

    internal static void RenderTable(
        Action<string> messageWriter,
        ICollection<CategoryTreeDto> categoryTrees
    )
    {
        messageWriter("Category Trees");

        var table = new ConsoleTables.ConsoleTable(
            nameof(CategoryTreeDto.Id),
            nameof(CategoryTreeDto.Name),
            nameof(CategoryTreeDto.CategoryTaxonomy),
            nameof(CategoryTreeDto.ChildCategoryCount),
            nameof(CategoryTreeDto.ProductCount)
        );

        foreach (var entity in categoryTrees)
        {
            table.AddRow(
                entity.Id,
                entity.Name,
                entity.CategoryTaxonomy?.Name ?? "n/a",
                entity.ChildCategoryCount(),
                entity.ProductCount()
            );
        }
        ;

        messageWriter(table.ToString());
    }
}
