namespace PimApi.ConsoleApp.Queries;

public interface IQueryWithTopSkip
{
    int? Top { get; }
    int? Skip { get; }
}
