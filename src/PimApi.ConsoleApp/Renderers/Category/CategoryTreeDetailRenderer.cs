using PimApi.ConsoleApp.Renderers.Product;
using PimApi.ConsoleApp.Renderers.Property;

namespace PimApi.ConsoleApp.Renderers.Category;

internal class CategoryTreeDetailRenderer : IApiResponseMessageRenderer
{
    public static readonly IApiResponseMessageRenderer Default = new CategoryTreeDetailRenderer();

    public async Task Render(
        ApiResponseMessage apiResponseMessage,
        IJsonSerializer jsonSerializer,
        Action<string> messageWriter
    )
    {
        var entity = await apiResponseMessage.GetDataAsync<CategoryTreeDto>(jsonSerializer);

        entity.WriteBaseEntityInfo(messageWriter);
        messageWriter(
            $"{nameof(CategoryTreeDto.CategoryTaxonomyId)} = {entity.CategoryTaxonomyId.DisplayValue()}"
        );
        messageWriter($"{nameof(CategoryTreeDto.ParentId)} = {entity.ParentId.DisplayValue()}");
        messageWriter($"{nameof(CategoryTreeDto.Name)} = {entity.Name.DisplayValue()}");
        messageWriter($"{nameof(CategoryTreeDto.UrlSegment)} = {entity.UrlSegment.DisplayValue()}");
        messageWriter(
            $"{nameof(CategoryTreeDto.Description)} = {entity.Description.DisplayValue()}"
        );
        messageWriter($"{nameof(CategoryTreeDto.ActivateOn)} = {entity.ActivateOn.DisplayValue()}");
        messageWriter(
            $"{nameof(CategoryTreeDto.DeactivateOn)} = {entity.DeactivateOn.DisplayValue()}"
        );
        messageWriter(
            $"{nameof(CategoryTreeDto.MetaDescription)} = {entity.MetaDescription.DisplayValue()}"
        );
        messageWriter(
            $"{nameof(CategoryTreeDto.MetaKeywords)} = {entity.MetaKeywords.DisplayValue()}"
        );
        messageWriter(
            $"{nameof(CategoryTreeDto.OpenGraphImageId)} = {entity.OpenGraphImageId.DisplayValue()}"
        );
        messageWriter(
            $"{nameof(CategoryTreeDto.OpenGraphTitle)} = {entity.OpenGraphTitle.DisplayValue()}"
        );
        messageWriter($"{nameof(CategoryTreeDto.PageTitle)} = {entity.PageTitle.DisplayValue()}");
        messageWriter(
            $"{nameof(CategoryTreeDto.ChildCategoryCount)} = {entity.ChildCategoryCount().DisplayValue()}"
        );
        messageWriter(
            $"{nameof(CategoryTreeDto.ProductCount)} = {entity.ProductCount().DisplayValue()}"
        );
        messageWriter(
            $"{nameof(CategoryTreeDto.PropertyCount)} = {entity.PropertyCount().DisplayValue()}"
        );

        if (entity.CategoryTaxonomy is not null)
        {
            messageWriter(
                $"{nameof(CategoryTreeDto.CategoryTaxonomy.Name)} = {entity.CategoryTaxonomy.Name.DisplayValue()}"
            );
        }

        if (entity.Parent is not null)
        {
            messageWriter(
                $"{nameof(CategoryTreeDto.Parent.Name)} = {entity.Parent.Name.DisplayValue()}"
            );
        }

        if (entity.ChildCategoryTrees.Count > 0)
        {
            messageWriter.Invoke(string.Empty);
            CategoryTreeListRenderer.RenderTable(messageWriter, entity.ChildCategoryTrees);
        }

        if (entity.Products.Count > 0)
        {
            messageWriter.Invoke(string.Empty);
            ProductListRenderer.RenderProductsTable(
                messageWriter,
                entity.Products.Select(o => o.Product).OfType<ProductDto>().ToArray()
            );
        }

        if (entity.Properties.Count > 0)
        {
            messageWriter.Invoke(string.Empty);
            PropertyListRenderer.RenderTable(
                messageWriter,
                entity.Properties.Select(o => o.Property).OfType<PropertyDto>().ToArray()
            );
        }
    }
}
