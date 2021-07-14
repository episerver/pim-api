using System;
using System.Threading.Tasks;

namespace PimApi.ConsoleApp
{
    /// <summary>
    /// Writes API response for Console Demo
    /// </summary>
    public interface IApiResponseMessageRenderer
    {
        Task Render(
            ApiResponseMessage apiResponseMessage,
            IJsonSerializer jsonSerializer,
            Action<string> messageWriter);
    }
}