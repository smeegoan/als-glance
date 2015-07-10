using System.Web.Http.Filters;
using ALS.Glance.Api.Security.Filters;

namespace ALS.Glance.Api
{
    public static class FilterConfig
    {
        /// <summary>
        /// Registers global filters into a http filter collection
        /// </summary>
        /// <param name="filters">The http filter collection</param>
        public static void RegisterGlobalFilters(this HttpFilterCollection filters)
        {
            filters.Add(new IdentityBearerAuthenticationAttribute());
        }

    }
}