using System.ComponentModel.DataAnnotations;

namespace ALS.Glance.Api.Models
{
    /// <summary>
    /// Represents a new authorization request
    /// </summary>
    public class AuthorizationRequest
    {
        /// <summary>
        /// The client application identifier that should be known to the API
        /// </summary>
   
        public string ApplicationId { get; set; }

        /// <summary>
        /// The email of the user
        /// </summary>
 
        public string UserName { get; set; }

        /// <summary>
        /// The user's password
        /// </summary>
  
        public string Password { get; set; }

        /// <summary>
        /// The associated authorization
        /// </summary>
        public Authorization Authorization { get; set; }
    }
}