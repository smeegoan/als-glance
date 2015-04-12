namespace ALS.Glance.Models.Security.Interfaces
{
    public interface IIdentityUserRole<TKey>
    {
        TKey RoleId { get; set; }
        TKey UserId { get; set; }
    }
}