using System;
using System.Runtime.Serialization;
using System.Security;
using ALS.Glance.UoW.Core.Properties;

namespace ALS.Glance.UoW.Core.Exceptions
{
    /// <summary>
    /// Exception to be used when the scope is not defined for the <see cref="ScopeEnabledUnitOfWork"/>
    /// </summary>
    [Serializable]
    public class UndefinedScopeException : UnitOfWorkException
    {
        public UndefinedScopeException() : base(Resources.UndefinedScopeExceptionMessage) { }

        public UndefinedScopeException(Exception innerException)
            : base(Resources.UndefinedScopeExceptionMessage, innerException) { }

        public UndefinedScopeException(string message, Exception innerException = null)
            : base(message, innerException) { }

        [SecuritySafeCritical]
        protected UndefinedScopeException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}