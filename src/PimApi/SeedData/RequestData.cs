using Optimizely.PIM.Data.V1;

namespace PimApi.SeedData;

public static class RequestData
{
    public static IReadOnlyCollection<Product> GetProducts()
    {
        return Enumerable
            .Range(1, 100)
            .Select(index =>
            {
                var product = new Product
                {
                    ProductNumber = $"ProductNumber-{index}",
                    ProductTitle = $"ProductTitle-{index}",
                    UrlSegment = $"UrlSegment-{index}",
                    ProductTemplate = "Starter Template"
                };
                product.PropertyBag.Add("Dropdown", "D1");
                product.PropertyBag.Add("NewMultiselect", "M1~M2");

                product.ProductAssets.Add(
                    new ProductAsset
                    {
                        AssetFolder = $"Folder-{index}",
                        AssetName = $"AssetName-{index}"
                    }
                );

                product.ProductRelatedProducts.Add(
                    new ProductRelatedProduct
                    {
                        ProductNumber = $"Child-{index}",
                        RelationshipType = "RelationshipType"
                    }
                );

                var productCategory = new ProductCategory { CategoryTree = $"Categories{index}" };
                productCategory.Categories.Add("Category 1");
                productCategory.Categories.Add("Category 2");
                product.ProductCategories.Add(productCategory);

                return product;
            })
            .ToArray();
    }
}
