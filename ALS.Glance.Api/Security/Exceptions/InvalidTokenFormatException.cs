using System;

namespace ALS.Glance.Api.Security.Exceptions
{
    public class InvalidTokenFormatException : ApiSecurityBusinessException
    {
        private const string DefaultErrorMessage = "Invalid token format. See inner exception for details.";

        /// <summary>
        /// 
        /// </summary>
        public InvalidTokenFormatException()
            : this(DefaultErrorMessage) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public InvalidTokenFormatException(string message)
            : base(message) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="innerException"></param>
        public InvalidTokenFormatException(Exception innerException)
            : base(DefaultErrorMessage, innerException) { }
    }
}