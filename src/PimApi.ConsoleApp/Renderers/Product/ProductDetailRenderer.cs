using PimApi.ConsoleApp.Renderers.Asset;
using PimApi.ConsoleApp.Renderers.Category;

namespace PimApi.ConsoleApp.Renderers.Product;

internal class ProductDetailRenderer : IApiResponseMessageRenderer
{
    public static readonly IApiResponseMessageRenderer Default = new ProductDetailRenderer();

    public async Task Render(
        ApiResponseMessage apiResponseMessage,
        IJsonSerializer jsonSerializer,
        Action<string> messageWriter)
    {
        var product = await apiResponseMessage.GetDataAsync<ProductDto>(jsonSerializer);

        RenderProductDetails(messageWriter, product);
    }

    internal static void RenderProductDetails(Action<string> messageWriter, ProductDto product)
    {
        product.WriteBaseEntityInfo(messageWriter);
        messageWriter($"{nameof(ProductDto.ProductNumber)} = {product.ProductNumber}");
        messageWriter($"{nameof(ProductDto.ProductTitle)} = {product.ProductTitle}");
        messageWriter($"{nameof(ProductDto.Status)} = {product.Status}");
        messageWriter($"{nameof(ProductDto.UrlSegment)} = {product.UrlSegment.DisplayValue()}");
        messageWriter($"{nameof(ProductDto.CurrentVersionNumber)} = {product.CurrentVersionNumber}");
        messageWriter($"{nameof(ProductDto.PublishedVersionNumber)} = {product.PublishedVersionNumber.DisplayValue()}");
        messageWriter($"{nameof(ProductDto.DeactivateOn)} = {product.DeactivateOn.DisplayValue()}");
        messageWriter($"{nameof(ProductDto.TemplateName)} = {product.TemplateName}");
        messageWriter($"{nameof(ProductDto.TemplateId)} = {product.TemplateId.DisplayValue()}");
        messageWriter($"{nameof(ProductDto.IsVariantParent)} = {product.IsVariantParent.DisplayValue()}");
        messageWriter($"{nameof(ProductDto.DefaultChildProductId)} = {product.DefaultChildProductId.DisplayValue()}");
        messageWriter($"{nameof(ProductDto.PercentComplete)} = {product.PercentComplete.DisplayValue()}");
        messageWriter($"{nameof(ProductDto.PercentRecommendedComplete)} = {product.PercentRecommendedComplete.DisplayValue()}");
        messageWriter($"{nameof(ProductDto.PrimaryCategoryTreeId)} = {product.PrimaryCategoryTreeId.DisplayValue()}");
        messageWriter($"{nameof(ProductDto.PrimaryCategoryName)} = {product.PrimaryCategoryName.DisplayValue()}");
        messageWriter($"{nameof(ProductDto.PrimaryImageAssetId)} = {product.PrimaryImageAssetId.DisplayValue()}");
        messageWriter($"{nameof(ProductDto.TagList)} = {product.TagList.DisplayValue()}");
        messageWriter($"{nameof(ProductDto.VariantPropertyIdList)} = {product.VariantPropertyIdList.DisplayValue()}");

        foreach (var (name, value) in product.PropertyBag)
        {
            messageWriter($"{name} = {value.DisplayValue()}");
        }

        if (product.ProductAssets.Count > 0)
        {
            messageWriter(string.Empty);
            AssetListRenderer.RenderTable(messageWriter, product.ProductAssets
                .Select(o => o.Asset)
                .OfType<AssetDto>()
                .ToList());
        }

        if (product.CategoryTrees.Count > 0)
        {
            messageWriter(string.Empty);
            CategoryTreeListRenderer.RenderTable(messageWriter, product.CategoryTrees
                .Select(o => o.CategoryTree)
                .OfType<CategoryTreeDto>()
                .ToList());
        }

        if (product.ProductRelatedProducts.Count > 0)
        {
            messageWriter(string.Empty);
            RelatedProductRendererRender(
                messageWriter,
                nameof(product.ProductRelatedProducts),
                product.ProductRelatedProducts);
        }

        if (product.ProductRelatedProductsOf.Count > 0)
        {
            messageWriter(string.Empty);
            RelatedProductRendererRender(
                messageWriter,
                nameof(product.ProductRelatedProductsOf),
                product.ProductRelatedProductsOf);
        }
    }

    private static void RelatedProductRendererRender(
            Action<string> messageWriter,
            string heading,
            ICollection<ProductRelatedProductDto> productRelatedProducts)
    {
        messageWriter(heading);
        var table = new ConsoleTables.ConsoleTable(
            nameof(ProductDto.ProductNumber),
            nameof(ProductRelationshipDto.Name));

        foreach (var relatedProduct in productRelatedProducts)
        {
            table.AddRow(
                relatedProduct.RelateProduct?.ProductNumber,
                relatedProduct.ProductRelationship?.Name);
        }

        messageWriter(table.ToString());
    }
}