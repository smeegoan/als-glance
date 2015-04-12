namespace ALS.Glance.Api.Security
{
    public class ServiceRoles
    {
        public static readonly string[] DefaultRoles = { User, UserExternal };

        public const string Admin = "admin";
        public const string Application = "application";
        public const string User = "user";
        public const string UserExternal = "user.external";
    }
}
