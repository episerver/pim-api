using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PimApi
{
    public class ApiResponseMessage : IDisposable
    {
        private readonly Task<HttpResponseMessage> apiResponse;
        private object? data;
        private bool deserializedData;
        private static readonly Exception deserializeException =
            new("Unable to deserialize the response stream");

        public ApiResponseMessage(Task<HttpResponseMessage> apiResponse) =>
            this.apiResponse = apiResponse;

        public Task<HttpResponseMessage> GetHttpResponseMessage() =>
            apiResponse;

        public async Task<bool> IsSuccessful()
        {
            await EnsureCompleteRequest();

            return apiResponse.Result.IsSuccessStatusCode;
        }

        /// <summary>
        /// This is the preferred and most performant way to get response content. 
        /// <para>MUST only be called once as stream is disposed once read.</para>
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="jsonSerializer"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Thrown when unable to read data</exception>
        public async Task<TData> GetDataAsync<TData>(IJsonSerializer jsonSerializer)
            where TData : class
        {
            if (data is not null) { return (TData)data; }
            if (deserializedData) { throw deserializeException; }

            deserializedData = true;
            await EnsureCompleteRequest();

            if (!apiResponse.Result.IsSuccessStatusCode)
            {
                throw new Exception($"Unable to get data from response due to status code: {(int)apiResponse.Result.StatusCode}");
            }

            using var stream = await apiResponse.Result.Content.ReadAsStreamAsync();

            data = await jsonSerializer.DeserializeAsync<TData>(stream) ?? throw deserializeException;

            return (TData)data;
        }

        private async Task EnsureCompleteRequest()
        {
            if (apiResponse.IsCompleted) { return; }

            await apiResponse;
        }

        public void Dispose()
        {
            if (!apiResponse.IsCompleted) { return; }

            apiResponse.Result.Dispose();
        }

        public static implicit operator ApiResponseMessage(Task<HttpResponseMessage> message) => new(message);
    }
}
