using System.ComponentModel.DataAnnotations;

namespace ALS.Glance.Api.Models
{
    /// <summary>
    /// Represents a refresh authorization request
    /// </summary>
    public class AuthorizationRefresh
    {
        /// <summary>
        /// The client application identifier known by the API
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// The refresh token
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// The associated authorization
        /// </summary>
        public Authorization Authorization { get; set; }
    }
}