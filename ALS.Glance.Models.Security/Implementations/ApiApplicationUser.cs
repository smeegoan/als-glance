using System.Collections.Generic;

namespace ALS.Glance.Models.Security.Implementations
{
    public class ApiApplicationUser : ApplicationUser
    {

        public ApiApplicationUser()
        {
            ApiAuthenticationTokens = new HashSet<ApiAuthenticationToken>();
        }

        public virtual ICollection<ApiAuthenticationToken> ApiAuthenticationTokens
        {
            get;
            set;
        }
    }
}
