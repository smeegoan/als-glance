using System;
using System.Runtime.Serialization;
using System.Security;
using ALS.Glance.UoW.Core.Properties;

namespace ALS.Glance.UoW.Core.Exceptions
{
    /// <summary>
    /// Exception to be used when an entity with the same identifier already exists
    /// </summary>
    [Serializable]
    public class DuplicatedIdentifierException : UnitOfWorkException
    {
        public DuplicatedIdentifierException() : base(Resources.DuplicatedIdentifierExceptionMessage) { }

        public DuplicatedIdentifierException(Exception innerException)
            : base(Resources.DuplicatedIdentifierExceptionMessage, innerException) { }

        public DuplicatedIdentifierException(string message, Exception innerException = null)
            : base(message, innerException) { }

        [SecuritySafeCritical]
        protected DuplicatedIdentifierException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}