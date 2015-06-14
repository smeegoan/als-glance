using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace ALS.Glance.Api.Helpers.ODataInterfaces
{
    /// <summary>
    /// HTTP Put OData interfaces
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class ODataDelete
    {
        /// <summary>
        /// Using a long as a key
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public interface WithKey<in TKey>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="key"></param>
            /// <param name="ct"></param>
            /// <returns></returns>
            Task<IHttpActionResult> Delete([FromODataUri] TKey key, CancellationToken ct);
        }

        /// <summary>
        /// Using a long as a key
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public interface WithKey<in TKey1, in TKey2>
        {
            Task<IHttpActionResult> Delete([FromODataUri] TKey1 key1, [FromODataUri] TKey2 key2, CancellationToken ct);
        }
    }
}