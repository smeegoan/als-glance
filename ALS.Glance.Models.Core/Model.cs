namespace ALS.Glance.Models.Core
{
    /// <summary>
    /// The model class with an associated unique identifier
    /// </summary>
    /// <typeparam name="TId">The unique model identifier type</typeparam>
    public abstract class Model<TId> : Model, IModel<TId>
    {
        public virtual TId Id { get; set; }
    }

    /// <summary>
    /// The model class
    /// </summary>
    public abstract class Model : IModel
    {

    }
}