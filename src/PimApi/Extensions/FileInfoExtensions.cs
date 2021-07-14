using System;
using System.IO;
using System.Threading.Tasks;

namespace PimApi.Extensions
{
    public static class FileInfoExtensions
    {
        /// <summary>
        /// Creates connection information from a given file in the format of
        /// <para>
        /// {
        ///   "AppKey": "",
        ///   "AppSecret": "",
        /// }
        /// </para>
        /// </summary>
        /// <param name="connectionFile"></param>
        /// <param name="jsonSerializer"></param>
        public static async Task<ConnectionInformation> GetConnectionInformation(this FileInfo connectionFile, IJsonSerializer jsonSerializer)
        {
            if (!connectionFile.Exists) { throw new Exception($"Unable to read file {connectionFile.FullName}, it does not exist!;"); }

            using var fileReader = connectionFile.OpenRead();
            var connection = await jsonSerializer.DeserializeAsync<ConnectionInformation>(fileReader);

            connection?.EnsureConnectionInformation();

            return connection ?? throw new Exception("Unable to create connection information from given file!");
        }
    }
}
