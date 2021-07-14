using System.Runtime.Serialization;

namespace PimApi.Entities
{
    /// <summary>
    /// Represents entity data stored in PIM DB table with an Http Endpoint
    /// </summary>
    public abstract class BaseEntityDtoWithEndpoint : BaseEntityDto
    {
        /// <summary>
        /// API Endpoint for HTTP requests
        /// </summary>
        [IgnoreDataMember]
        public abstract string EntityUrlBase { get; }
    }
}