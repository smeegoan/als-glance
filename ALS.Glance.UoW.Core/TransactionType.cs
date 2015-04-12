namespace ALS.Glance.UoW.Core
{
    /// <summary>
    /// The transaction type to be used
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// Create a new transaction or use an existing one
        /// </summary>
        Required,
        /// <summary>
        /// Always create a new transaction
        /// </summary>
        RequiresNew,
        /// <summary>
        /// Does not open a new transaction but uses an existing one if available
        /// </summary>
        NotRequired
    }
}
