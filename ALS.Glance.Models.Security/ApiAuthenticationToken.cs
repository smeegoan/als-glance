using System;
using System.Collections.Generic;
using ALS.Glance.Models.Security.Implementations;

namespace ALS.Glance.Models.Security
{
    public class ApiAuthenticationToken
    {
        public ApiAuthenticationToken()
        {
            ApiAuthenticationAccessTokens= new HashSet<ApiAuthenticationAccessToken>();
        }

        /// <summary>
        /// The application id
        /// </summary>
        public string ApiApplicationId { get; set; }

        /// <summary>
        /// The associated application
        /// </summary>
        public virtual ApplicationUser ApiApplication { get; set; }

        /// <summary>
        /// The user id
        /// </summary>
        public string BaseApiUserId { get; set; }

        /// <summary>
        /// The associated user
        /// </summary>
        public virtual ApiUser BaseApiUser { get; set; }

        /// <summary>
        /// The generated refresh token
        /// </summary>
        public Guid RefreshToken { get; set; }

        public virtual ICollection<ApiAuthenticationAccessToken> ApiAuthenticationAccessTokens { get; set; }
    }
}
