using PimApi.ConsoleApp.Renderers.Miscellaneous;

namespace PimApi.ConsoleApp.Queries.Miscellaneous;

[Display(
    GroupName = nameof(Miscellaneous),
    Order = 910,
    Description = "Shows product templates ordered by display sequence"
)]
public class GetTemplates : IQuery, IQueryWithTopSkip, IQueryWithMessageRenderer
{
    public int? Top { get; set; }

    public int? Skip { get; set; }

    public IApiResponseMessageRenderer MessageRenderer => TemplateListRenderer.Default;

    public ApiResponseMessage Execute(HttpClient pimApiClient) =>
        pimApiClient.GetAsync(
            new ODataQuery<TemplateDto>
            {
                Count = true,
                Top = this.GetTopValue(),
                Skip = this.GetSkipValue(),
                OrderBy = nameof(TemplateDto.DisplaySequence),
                Expand =
                    $"{nameof(TemplateDto.ParentTemplate)}($select=name),"
                    + $"{nameof(TemplateDto.ChildrenTemplates)}($count=true),"
                    + $"{nameof(TemplateDto.TemplatePropertyGroups)}($count=true)"
            }
        );
}
