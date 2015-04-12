using System.Web.Http;

namespace ALS.Glance.Api.Security.Filters
{
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        private ApiAuthorizeAttribute(string[] roles)
        {
            Roles = string.Join(",", roles); 
        }
        public ApiAuthorizeAttribute() : this(new string[0]) { }

        public ApiAuthorizeAttribute(string role) : this(new[] { role }) { }

        public ApiAuthorizeAttribute(string role1, string role2) : this(new[] { role1, role2 }) { }

        public ApiAuthorizeAttribute(string role1, string role2, string role3) : this(new[] { role1, role2, role3}) { }

        public ApiAuthorizeAttribute(string role1, string role2, string role3, string role4) : this(new[] { role1, role2, role3, role4 }) { }

    }
}