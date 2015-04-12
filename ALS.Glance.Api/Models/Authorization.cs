namespace ALS.Glance.Api.Models
{
    /// <summary>
    /// Represents an authorization object that contains the required information
    /// for using API resources that demand authentication
    /// </summary>
    public class Authorization
    {
        /// <summary>
        /// Represents an access token that should be passed in the HTTP Header Authorization
        /// (for example: PUT Api/PharmacyCards/{id} Authorization: Bearer {AcessToken}) 
        /// when invoking API resources that require authentication. This will be used as a temporary
        /// session identifier with a time to live of about an hour.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Represents a refresh token to be used to get a new access token when the current expires.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Indicates the time to live, in minutes, of the current token.
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// The associated user id
        /// </summary>
        public string UserName { get; set; }
    }
}