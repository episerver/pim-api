using PimApi.ConsoleApp.Renderers.Product;
using PimApi.Extensions;

namespace PimApi.ConsoleApp.Queries.Product;

[Display(
    GroupName = nameof(Product),
    Order = 12,
    Description = "Searches products by createdon|modifiedon|lastpublished in last N days (default: 60)"
)]
public class SearchByDateFieldLastNDays : IQuery, IQueryWithMessageRenderer, IQueryWithTopSkip
{
    private readonly DateTimeProvider dateTimeProvider;

    public SearchByDateFieldLastNDays()
        : this(DateTimeProvider.Default) { }

    public SearchByDateFieldLastNDays(DateTimeProvider dateTimeProvider) =>
        this.dateTimeProvider = dateTimeProvider;

    private const int DefaultPreviousDays = 60;

    public int? PreviousDays { get; set; }

    public int? Top { get; set; }

    public int? Skip { get; set; }

    public string? DateField { get; set; }

    IApiResponseMessageRenderer IQueryWithMessageRenderer.MessageRenderer =>
        ProductListRenderer.Default;

    public ApiResponseMessage Execute(HttpClient pimApiClient)
    {
        var previousDays =
            this.PreviousDays
            ?? Program.ReadValue($"Please enter previous days:", DefaultPreviousDays);

        var dateField =
            this.DateField
            ?? Program.ReadValue($"Please enter date field for filtering", "createdon");

        var request = new ODataQuery<ProductDto>
        {
            Top = this.GetTopValue(),
            Skip = this.GetSkipValue(),
            Count = true,
            OrderBy = $"{nameof(ProductDto.ModifiedOn)} desc",
            Filter = $"{dateField} ge {this.dateTimeProvider.GetPreviousDate(previousDays)}"
        };

        return pimApiClient.GetAsync(request);
    }
}
