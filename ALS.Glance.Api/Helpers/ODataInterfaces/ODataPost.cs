using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace ALS.Glance.Api.Helpers.ODataInterfaces
{
    /// <summary>
    /// HTTP Post OData interfaces
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public interface ODataPost<in TEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<IHttpActionResult> Post(TEntity entity, CancellationToken ct);
    }
}