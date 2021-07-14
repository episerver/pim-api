using PimApi.Entities;
using System;
using System.Threading.Tasks;

namespace PimApi.ConsoleApp.Renderers.Miscellaneous
{
    internal class TemplateDetailRenderer : IApiResponseMessageRenderer
    {
        public static readonly IApiResponseMessageRenderer Default = new TemplateDetailRenderer();

        public async Task Render(
            ApiResponseMessage apiResponseMessage,
            IJsonSerializer jsonSerializer,
            Action<string> messageWriter)
        {
            var entity = await apiResponseMessage.GetDataAsync<TemplateDto>(jsonSerializer);

            entity.WriteBaseEntityInfo(messageWriter);
            messageWriter($"{nameof(TemplateDto.Name)} = {entity.Name}");

            foreach (var (name, value) in entity.PropertyBag)
            {
                messageWriter($"{name} = {value.DisplayValue()}");
            }
        }
    }
}