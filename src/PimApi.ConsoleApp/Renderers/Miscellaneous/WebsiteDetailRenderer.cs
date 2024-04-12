namespace PimApi.ConsoleApp.Renderers.Miscellaneous;

internal class WebsiteDetailRenderer : IApiResponseMessageRenderer
{
    public static readonly IApiResponseMessageRenderer Default = new WebsiteDetailRenderer();

    public async Task Render(
        ApiResponseMessage apiResponseMessage,
        IJsonSerializer jsonSerializer,
        Action<string> messageWriter)
    {
        var entity = await apiResponseMessage.GetDataAsync<WebsiteDto>(jsonSerializer);

        entity.WriteBaseEntityInfo(messageWriter);
        messageWriter($"{nameof(WebsiteDto.Name)} = {entity.Name}");

        foreach (var (name, value) in entity.PropertyBag)
        {
            messageWriter($"{name} = {value.DisplayValue()}");
        }
    }
}