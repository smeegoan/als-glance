using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;

namespace ALS.Glance.Api.Helpers.ODataInterfaces
{
    /// <summary>
    /// HTTP Get OData interfaces
    /// </summary>
    public static class ODataGet<TEntity>
    {
        /// <summary>
        /// Using T as a key
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public interface WithKey<in TKey>
        {
            /// <summary>
            /// Gets an <see cref="IQueryable{T}"/> of the given entity
            /// </summary>
            /// <returns></returns>
            [EnableQuery]
            IQueryable<TEntity> Get();

            /// <summary>
            /// Gets an <see cref="IQueryable{T}"/> of the given entity
            /// </summary>
            /// <returns></returns>
            [EnableQuery]
            Task<IHttpActionResult> Get([FromODataUri] TKey key, CancellationToken ct);
        }


        /// <summary>
        /// Using T as a key
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public interface WithKey<in TKey1, in TKey2>
        {
            /// <summary>
            /// Gets an <see cref="IQueryable{T}"/> of the given entity
            /// </summary>
            /// <returns></returns>
            [EnableQuery]
            IQueryable<TEntity> Get();

            /// <summary>
            /// Gets an <see cref="IQueryable{T}"/> of the given entity
            /// </summary>
            /// <returns></returns>
            [EnableQuery]
            Task<IHttpActionResult> Get([FromODataUri] TKey1 key1, [FromODataUri] TKey2 key2, CancellationToken ct);
        }
        /// <summary>
        /// Using T as a key
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public interface WithKey<in TKey1, in TKey2, in TKey3>
        {
            /// <summary>
            /// Gets an <see cref="IQueryable{T}"/> of the given entity
            /// </summary>
            /// <returns></returns>
            [EnableQuery]
            IQueryable<TEntity> Get();

            /// <summary>
            /// Gets an <see cref="IQueryable{T}"/> of the given entity
            /// </summary>
            /// <returns></returns>
            [EnableQuery]
            Task<IHttpActionResult> Get([FromODataUri] TKey1 applicationId, [FromODataUri] TKey2 key2, [FromODataUri] TKey3 key3, CancellationToken ct);
        }


        // ReSharper disable once InconsistentNaming
        public interface WithKeyAndOptions<in TKey>
        {
            /// <summary>
            /// Gets an <see cref="IQueryable{T}"/> of the given entity
            /// </summary>
            /// <returns></returns>
            [EnableQuery]
            IQueryable<TEntity> Get(ODataQueryOptions<TEntity> options);

            /// <summary>
            /// Gets an <see cref="IQueryable{T}"/> of the given entity
            /// </summary>
            /// <returns></returns>
            [EnableQuery]
            Task<IHttpActionResult> Get([FromODataUri] TKey key, ODataQueryOptions<TEntity> options, CancellationToken ct);
        }
        // ReSharper disable once InconsistentNaming
        public interface WithKeyAndOptions<in TKey1, in TKey2>
        {
            /// <summary>
            /// Gets an <see cref="IQueryable{T}"/> of the given entity
            /// </summary>
            /// <returns></returns>
            [EnableQuery]
            IQueryable<TEntity> Get(ODataQueryOptions<TEntity> options);

            /// <summary>
            /// Gets an <see cref="IQueryable{T}"/> of the given entity
            /// </summary>
            /// <returns></returns>
            [EnableQuery]
            Task<IHttpActionResult> Get([FromODataUri] TKey1 key1, [FromODataUri] TKey2 key2, ODataQueryOptions<TEntity> options, CancellationToken ct);
        }
    }
}