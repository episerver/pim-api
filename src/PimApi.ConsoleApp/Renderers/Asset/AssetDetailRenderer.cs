using PimApi.Entities;
using System;
using System.Threading.Tasks;

namespace PimApi.ConsoleApp.Renderers.Asset
{
    internal class AssetDetailRenderer : IApiResponseMessageRenderer
    {
        public static readonly IApiResponseMessageRenderer Default = new AssetDetailRenderer();

        public async Task Render(
            ApiResponseMessage apiResponseMessage,
            IJsonSerializer jsonSerializer,
            Action<string> messageWriter)
        {
            var dto = await apiResponseMessage.GetDataAsync<AssetDto>(jsonSerializer);

            dto.WriteBaseEntityInfo(messageWriter);
            messageWriter($"{nameof(AssetDto.Name)} = {dto.Name.DisplayValue()}");
            messageWriter($"{nameof(AssetDto.Type)} = {dto.Type.DisplayValue()}");
            messageWriter($"{nameof(AssetDto.AssetType)} = {dto.AssetType.DisplayValue()}");
            messageWriter($"{nameof(AssetDto.AssetFileName)} = {dto.AssetFileName.DisplayValue()}");
            messageWriter($"{nameof(AssetDto.Description)} = {dto.Description.DisplayValue()}");
            messageWriter($"{nameof(AssetDto.IsArchived)} = {dto.IsArchived.DisplayValue()}");
            messageWriter($"{nameof(AssetDto.IsExternal)} = {dto.IsExternal.DisplayValue()}");
            messageWriter($"{nameof(AssetDto.InternalFileName)} = {dto.InternalFileName.DisplayValue()}");
            messageWriter($"{nameof(AssetDto.ExternalFileUrl)} = {dto.ExternalFileUrl.DisplayValue()}");
            messageWriter($"{nameof(AssetDto.UrlSmall)} = {dto.UrlSmall.DisplayValue()}");
            messageWriter($"{nameof(AssetDto.UrlMedium)} = {dto.UrlMedium.DisplayValue()}");
            messageWriter($"{nameof(AssetDto.UrlLarge)} = {dto.UrlLarge.DisplayValue()}");
            messageWriter($"{nameof(AssetDto.AltText)} = {dto.AltText.DisplayValue()}");
            messageWriter($"{nameof(AssetDto.DocumentLanguage)} = {dto.DocumentLanguage.DisplayValue()}");
            messageWriter($"{nameof(AssetDto.FileExtension)} = {dto.FileExtension.DisplayValue()}");
            messageWriter($"{nameof(AssetDto.PreviousInternalFileName)} = {dto.PreviousInternalFileName.DisplayValue()}");
            messageWriter($"{nameof(AssetDto.PreviousFileExtension)} = {dto.PreviousFileExtension.DisplayValue()}");
            messageWriter($"{nameof(AssetDto.AssetTreeId)} = {dto.AssetTreeId.DisplayValue()}");
            messageWriter($"{nameof(AssetDto.CategoryCount)} = {dto.CategoryCount.DisplayValue()}");
            messageWriter($"{nameof(AssetDto.ProductCount)} = {dto.ProductCount.DisplayValue()}");
            messageWriter($"{nameof(AssetDto.CurrentAssetHash)} = {dto.CurrentAssetHash.DisplayValue()}");
            messageWriter($"{nameof(AssetDto.CurrentVersionNumber)} = {dto.CurrentVersionNumber.DisplayValue()}");
            messageWriter($"{nameof(AssetDto.TagList)} = {dto.TagList.DisplayValue()}");

            if (dto.ProductAssets.Count == 0) { return; }

            messageWriter(nameof(AssetDto.ProductAssets));
            foreach (var mapping in dto.ProductAssets)
            {
                if (mapping.Product is null) { continue; }

                messageWriter($"\t{mapping.Product!.ProductNumber}");
            }
        }
    }
}