using System;
using System.Runtime.Serialization;
using System.Security;
using ALS.Glance.UoW.Core.Properties;

namespace ALS.Glance.UoW.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when a commit to a <see cref="IUnitOfWork"/> fails.
    /// </summary>
    [Serializable]
    public class CommitException : UnitOfWorkException
    {
        public CommitException() : base(Resources.CommitExceptionMessage) { }

        public CommitException(Exception innerException)
            : base(Resources.CommitExceptionMessage, innerException) { }

        public CommitException(string message, Exception innerException = null)
            : base(message, innerException) { }

        [SecuritySafeCritical]
        protected CommitException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}