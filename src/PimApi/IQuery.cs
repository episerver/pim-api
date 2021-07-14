using System.Net.Http;

namespace PimApi
{
    public interface IQuery
    {
        ApiResponseMessage Execute(HttpClient pimApiClient);
    }
}
