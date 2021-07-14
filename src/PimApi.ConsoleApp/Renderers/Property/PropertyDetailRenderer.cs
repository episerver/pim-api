using PimApi.Entities;
using System;
using System.Threading.Tasks;

namespace PimApi.ConsoleApp.Renderers.Property
{
    internal class PropertyDetailRenderer : IApiResponseMessageRenderer
    {
        public static readonly IApiResponseMessageRenderer Default = new PropertyDetailRenderer();

        public async Task Render(
            ApiResponseMessage apiResponseMessage,
            IJsonSerializer jsonSerializer,
            Action<string> messageWriter)
        {
            var entity = await apiResponseMessage.GetDataAsync<PropertyDto>(jsonSerializer);

            entity.WriteBaseEntityInfo(messageWriter);
            messageWriter($"{nameof(PropertyDto.Name)} = {entity.Name}");

            foreach (var (name, value) in entity.PropertyBag)
            {
                messageWriter($"{name} = {value.DisplayValue()}");
            }
        }
    }
}