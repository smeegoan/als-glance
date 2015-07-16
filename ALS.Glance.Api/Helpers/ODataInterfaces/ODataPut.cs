using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;

namespace ALS.Glance.Api.Helpers.ODataInterfaces
{
    /// <summary>
    /// HTTP Put OData interfaces
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class ODataPut<TEntity> where TEntity : class
    {
        /// <summary>
        /// Using a TKey as a key
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public interface WithKey<in TKey>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="key"></param>
            /// <param name="update"></param>
            /// <param name="ct"></param>
            /// <returns></returns>
            Task<IHttpActionResult> Put([FromODataUri] TKey key, TEntity update, CancellationToken ct);
        }

        /// <summary>
        /// Using a TKey as a key
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public interface WithKey<in TKey1, in TKey2>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="key01"></param>
            /// <param name="key02"></param>
            /// <param name="update"></param>
            /// <param name="ct"></param>
            /// <returns></returns>
            Task<IHttpActionResult> Put([FromODataUri] TKey1 key01, [FromODataUri] TKey2 key02, TEntity update, CancellationToken ct);
        }

        // ReSharper disable once InconsistentNaming
        public interface WithKeyAndOptions<in TKey1, in TKey2>
        {
            Task<IHttpActionResult> Put([FromODataUri] TKey1 key1, [FromODataUri] TKey2 key2, TEntity update, ODataQueryOptions<TEntity> options, CancellationToken ct);
        }
    }
}