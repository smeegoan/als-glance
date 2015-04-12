
namespace ALS.Glance.Models.Security.Interfaces
{
    public interface IIdentityUserLogin<TKey>
    {
        string LoginProvider { get; set; }

        /// <summary>
        ///     Key representing the login for the provider
        /// </summary>
        string ProviderKey { get; set; }

        /// <summary>
        ///     User Id for the user who owns this login
        /// </summary>
        TKey UserId { get; set; }
    }
}
