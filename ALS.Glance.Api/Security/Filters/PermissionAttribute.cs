using System;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace ALS.Glance.Api.Security.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class PermissionAttribute : AuthorizeAttribute
    {
        public string MustOwn { get; set; }

        public string ClaimType { get; set; }

        public string Role { get; set; }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var user = actionContext.RequestContext.Principal as ClaimsPrincipal;

            if (user == null || !user.Identity.IsAuthenticated)
                return false;

            if (user.IsInRole(Security.Roles.Admin) || !user.IsInRole(Role))
                return true;

            var routeData = actionContext.Request.GetRouteData();
            var value = (string)routeData.Values[MustOwn];
            value = value.Substring(1, value.Length - 2);

            return user.HasClaim(ClaimType, value);
        }

    }
}