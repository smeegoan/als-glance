using System;
using ALS.Glance.UoW.Core.Properties;

namespace ALS.Glance.UoW.Core
{
    /// <summary>
    /// Indicates that this method or class should be executed inside a transaction
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class TransactionalAttribute : Attribute
    {
        /// <summary>
        /// The transaction type. By default will be assigned as <see cref="TransactionType"/>.Required
        /// </summary>
        public TransactionType TransactionType { get; set; }

        /// <summary>
        /// The transaction max duration. By default will be assigned as 1 minute
        /// </summary>
        public int TransactionDurationLimit { get; set; }

        /// <summary>
        /// If a TransactionScope should be used. By default will be set to false
        /// </summary>
        public bool UseGlobalTransaction { get; set; }

        /// <summary>
        /// The context type for which this annotation will be used
        /// </summary>
        public Type UnitOfWorkType { get; private set; }

        public TransactionalAttribute(Type unitOfWorkType)
        {
            if (unitOfWorkType == null)
                throw new ArgumentNullException("unitOfWorkType");
            if (!typeof(IUnitOfWork).IsAssignableFrom(unitOfWorkType))
                throw new ArgumentException(Resources.TransactionalAttributeInvalidUoWType, "unitOfWorkType");

            UnitOfWorkType = unitOfWorkType;
            TransactionDurationLimit = 1;
            TransactionType = TransactionType.Required;
            UseGlobalTransaction = false;
        }
    }
}