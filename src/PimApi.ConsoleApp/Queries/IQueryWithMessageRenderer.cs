namespace PimApi.ConsoleApp.Queries;

/// <summary>IQuery with a message writer if successful</summary>
public interface IQueryWithMessageRenderer : IQuery
{
    IApiResponseMessageRenderer MessageRenderer { get; }
}
