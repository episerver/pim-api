using System;

namespace PimApi.ConsoleApp.Queries
{
    public interface IQueryWithEntityId
    {
        Guid? Id { get; set; }
    }
}