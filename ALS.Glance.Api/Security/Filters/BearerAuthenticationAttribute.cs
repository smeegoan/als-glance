namespace ALS.Glance.Api.Security.Filters
{
    public abstract class BearerAuthenticationAttribute : ApiAuthenticationAttribute
    {
        /// <summary>
        /// Creates a new BearerAuthenticationAttribute with Scheme="Bearer"
        /// </summary>
        protected BearerAuthenticationAttribute()
            : base("Bearer") { }
    }
}