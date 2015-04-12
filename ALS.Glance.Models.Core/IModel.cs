namespace ALS.Glance.Models.Core
{
    /// <summary>
    /// The model interface with an associated unique identifier
    /// </summary>
    /// <typeparam name="TId">The unique model identifier type</typeparam>
    public interface IModel<TId> : IModel
    {
        /// <summary>
        /// The unique model identifier
        /// </summary>
        TId Id { get; set; }
    }

    /// <summary>
    /// The model interface
    /// </summary>
    public interface IModel
    {

    }
}