using ALS.Glance.Api.Properties;

namespace ALS.Glance.Api.Security.Exceptions
{
    public class ApiAuthorizationExpiredException : ApiSecurityBusinessException
    {
        /// <summary>
        /// Creates a new instance with default message
        /// </summary>
        public ApiAuthorizationExpiredException()
            : base(Resources.TokenExpiredErrorMessage) { }


    }
}