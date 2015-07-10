namespace ALS.Glance.Api.Security.Exceptions
{
    public class TokenNotFoundException : ApiSecurityBusinessException
    {
        private const string DefaultErrorMessage =
            "Could not match the token with an existing authentication. Please authenticate with credentials first";

        /// <summary>
        /// 
        /// </summary>
        public TokenNotFoundException()
            : this(DefaultErrorMessage) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public TokenNotFoundException(string message)
            : base(message) { }
    }
}