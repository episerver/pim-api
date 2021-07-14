using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PimApi.ConsoleApp.Renderers.Miscellaneous
{
    internal class UserChoiceRenderer : IApiResponseMessageRenderer
    {
        private readonly IList<IApiResponseMessageRenderer> apiRenderers;

        public UserChoiceRenderer(IList<IApiResponseMessageRenderer> apiRenderers) => this.apiRenderers = apiRenderers;

        public int? ChosenRenderer { get; set; }

        public Task Render(
            ApiResponseMessage apiResponseMessage,
            IJsonSerializer jsonSerializer,
            Action<string> messageWriter)
        {
            var index = 0;

            foreach (var renderer in this.apiRenderers)
            {
                messageWriter($"{index} {renderer.GetType().Name}");
                index++;
            }

            var chosenRenderer = this.ChosenRenderer;
            chosenRenderer ??= Program.ReadValue("Please enter renderer number:", 0);

            if (chosenRenderer >= this.apiRenderers.Count)
            {
                chosenRenderer = 0;
            }

            return this.apiRenderers[chosenRenderer.Value]
                .Render(apiResponseMessage, jsonSerializer, messageWriter);
        }
    }
}