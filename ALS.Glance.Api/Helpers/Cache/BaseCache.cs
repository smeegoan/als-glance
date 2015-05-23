using System.Web;

namespace ALS.Glance.Api.Helpers.Cache
{
    public abstract class BaseCache
    {
        protected static System.Web.Caching.Cache Cache = HttpRuntime.Cache;
    }
}