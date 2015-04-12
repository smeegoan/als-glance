using System;
using System.Runtime.Serialization;
using System.Security;
using ALS.Glance.UoW.Core.Properties;

namespace ALS.Glance.UoW.Core.Exceptions
{
    /// <summary>
    /// Exception to be used when the work can't be commited due to concurrency conflicts 
    /// </summary>
    [Serializable]
    public class ConcurrencyException : UnitOfWorkException
    {
        public ConcurrencyException() : base(Resources.ConcurrencyExceptionMessage) { }

        public ConcurrencyException(Exception innerException)
            : base(Resources.ConcurrencyExceptionMessage, innerException) { }

        public ConcurrencyException(string message, Exception innerException = null)
            : base(message, innerException) { }

        [SecuritySafeCritical]
        protected ConcurrencyException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}