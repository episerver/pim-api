namespace PimApi.ConsoleApp.Queries;

public interface IQueryWithParentId
{
    Guid? ParentId { get; }
}
