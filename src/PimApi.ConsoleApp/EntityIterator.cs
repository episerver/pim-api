using PimApi.ConsoleApp.Queries.Miscellaneous;
using PimApi.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace PimApi.ConsoleApp
{
    /// <summary>
    /// Entity Iterator will load all TEntity instances in efficient manner as possible on system resources
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class EntityIterator<TEntity>
        where TEntity : BaseEntityDtoWithEndpoint, new()
    {
        private readonly HttpClient apiClient;
        private readonly IJsonSerializer jsonSerializer;
        private readonly IApiResponseMessageRenderer apiResponseMessageRenderer;

        /// <summary>
        /// Runs tests assertions
        /// </summary>
        internal Func<IQuery, ApiResponseMessage, Task>? TestAssertions { get; set; }

        public EntityIterator(
            HttpClient apiClient,
            IJsonSerializer jsonSerializer,
            IApiResponseMessageRenderer apiResponseMessageRenderer)
        {
            this.apiClient = apiClient;
            this.jsonSerializer = jsonSerializer;
            this.apiResponseMessageRenderer = apiResponseMessageRenderer;
        }

        public int NumberOfRequests { get; private set; } = 0;

        public int TotalEntitiesForQuery { get; private set; } = 0;

        public async IAsyncEnumerable<TEntity> GetEntities(
            ODataQuery<TEntity> oDataQuery,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            // Top disables skip so remove it
            if (oDataQuery.Top is not null)
            {
                oDataQuery.Top = null;
            }

            var query = new GenericQuery(this.apiResponseMessageRenderer)
            {
                QueryText = oDataQuery
            };

            this.NumberOfRequests++;
            var result = query.Execute(this.apiClient);

            if (this.TestAssertions is not null)
            {
                await this.TestAssertions.Invoke(query, result);
            }

            var entityList = await result
                .GetDataAsync<ODataResponseCollection<TEntity>>(this.jsonSerializer);

            this.TotalEntitiesForQuery = entityList.Count!.Value;
            foreach (var entity in entityList.Value)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return entity;
            }

            var nextLink = entityList.NextLink;

            while (nextLink is not null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var nextResult = new ApiResponseMessage(this.apiClient.GetAsync(
                    nextLink, 
                    cancellationToken));

                if (this.TestAssertions is not null)
                {
                    await this.TestAssertions(query, nextResult);
                }

                var nextEntityList = await nextResult
                    .GetDataAsync<ODataResponseCollection<TEntity>>(this.jsonSerializer);

                foreach (var entity in nextEntityList!.Value)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return entity;
                }

                nextLink = nextEntityList?.NextLink;
                this.NumberOfRequests++;
            }
        }
    }
}