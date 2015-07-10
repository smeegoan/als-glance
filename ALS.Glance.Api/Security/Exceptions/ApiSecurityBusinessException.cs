using System;

namespace ALS.Glance.Api.Security.Exceptions
{/// <summary>
    /// Base exception class for api security exceptions
    /// </summary>
    public abstract class ApiSecurityBusinessException : Exception
    {
        /// <summary>
        /// Creates a new api security exception with the given message
        /// </summary>
        /// <param name="message">The exception message</param>
        protected ApiSecurityBusinessException(string message)
            : base(message) { }

        /// <summary>
        /// Creates a new api security exception with the given message and inner exception
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        protected ApiSecurityBusinessException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}