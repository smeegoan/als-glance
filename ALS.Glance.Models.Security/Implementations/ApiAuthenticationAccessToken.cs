using System;
using ALS.Glance.Models.Core;

namespace ALS.Glance.Models.Security.Implementations
{
    public class ApiAuthenticationAccessToken : Model<long>
    {
        /// <summary>
        /// The application id
        /// </summary>
        public string ApiApplicationId { get; set; }

        /// <summary>
        /// Tbe user id
        /// </summary>
        public string BaseApiUserId { get; set; }

        /// <summary>
        /// The parent authentication token
        /// </summary>
        public virtual ApiAuthenticationToken ApiAuthenticationToken { get; set; }

        /// <summary>
        /// The access token
        /// </summary>
        public Guid AccessToken { get; set; }

        /// <summary>
        /// The expiration date for the access token
        /// </summary>
        public DateTime ExpirationDate { get; set; }
    }
}
