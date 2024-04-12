namespace PimApi.ConsoleApp.Renderers.Miscellaneous;

internal class UserChoiceRenderer(IList<IApiResponseMessageRenderer> apiRenderers)
    : IApiResponseMessageRenderer
{
    public int? ChosenRenderer { get; set; }

    public Task Render(
        ApiResponseMessage apiResponseMessage,
        IJsonSerializer jsonSerializer,
        Action<string> messageWriter
    )
    {
        var index = 0;

        foreach (var renderer in apiRenderers)
        {
            messageWriter($"{index} {renderer.GetType().Name}");
            index++;
        }

        var chosenRenderer = this.ChosenRenderer;
        chosenRenderer ??= Program.ReadValue("Please enter renderer number:", 0);

        if (chosenRenderer >= apiRenderers.Count)
        {
            chosenRenderer = 0;
        }

        return apiRenderers[chosenRenderer.Value]
            .Render(apiResponseMessage, jsonSerializer, messageWriter);
    }
}
