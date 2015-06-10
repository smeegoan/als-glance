using System.Collections.Generic;

namespace ALS.Glance.Models.Security.Implementations
{
    public class ApiUser : IdentityUser
    {
        public ApiUser()
        {
            ApiAuthenticationTokens = new HashSet<ApiAuthenticationToken>();
        }

        public virtual ICollection<ApiAuthenticationToken> ApiAuthenticationTokens { get; set; }

    }
}
