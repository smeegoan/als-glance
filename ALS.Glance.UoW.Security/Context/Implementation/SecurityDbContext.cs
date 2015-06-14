using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using ALS.Glance.Models.Security;
using ALS.Glance.Models.Security.Implementations;
using ALS.Glance.UoW.Security.Context.Interfaces;

namespace ALS.Glance.UoW.Security.Context.Implementation
{
    public class SecurityDbContext : DbContext, ISecurityDbContext
    {

        public SecurityDbContext(string connection)
            : base(connection)
        {
            RequireUniqueEmail = true;
        }

        public bool RequireUniqueEmail { get; set; }

        public virtual DbSet<ApiAuthenticationAccessToken> ApiAuthenticationAccessToken { get; set; }

        public virtual DbSet<ApiAuthenticationToken> ApiAuthenticationToken { get; set; }

        public virtual DbSet<ApplicationUser> Applications { get; set; }

     
        public virtual DbSet<IdentityUser> IdentityUsers { get; set; }

        public virtual DbSet<IdentityRole> Roles { get; set; }

        public int Commit()
        {
            return SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUser>(cfg =>
            {
                cfg.ToTable("AspNetUsers");
                cfg.HasMany(u => u.Roles).WithRequired().HasForeignKey(ur => ur.UserId);
                cfg.HasMany(u => u.Claims).WithRequired().HasForeignKey(uc => uc.UserId);
                cfg.HasMany(u => u.Logins).WithRequired().HasForeignKey(ul => ul.UserId);
                cfg.Property(u => u.UserName)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnAnnotation("Index",
                        new IndexAnnotation(new IndexAttribute("UserNameIndex") { IsUnique = true }));
                cfg.Property(e => e.Description).HasMaxLength(512);
                cfg.MapCreatedAndUpdatedMeta();
            });

            modelBuilder.Entity<IdentityUserRole>(cfg =>
            {
                cfg.ToTable("AspNetUserRoles");
                cfg.HasKey(r => new { r.UserId, r.RoleId });
            });

            modelBuilder.Entity<IdentityUserLogin>(cfg =>
            {
                cfg.HasKey(l => new { l.LoginProvider, l.ProviderKey, l.UserId });
                cfg.ToTable("AspNetUserLogins");
            });

            modelBuilder.Entity<IdentityUserClaim>(cfg =>
            {
                cfg.ToTable("AspNetUserClaims");
            });

            modelBuilder.Entity<IdentityRole>(cfg =>
            {
                cfg.ToTable("AspNetRoles");
                cfg.Property(r => r.Name)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnAnnotation("Index",
                        new IndexAnnotation(new IndexAttribute("RoleNameIndex") { IsUnique = true }));
                cfg.HasMany(r => r.Users).WithRequired().HasForeignKey(ur => ur.RoleId);
            });

          
            modelBuilder.Entity<ApplicationUser>(cfg => cfg.ToTable("AspNetExtApplications"));

            modelBuilder.Entity<ApiAuthenticationToken>(cfg =>
            {
                cfg.ToTable("AspNetExtApiAuthenticationTokens")
                    .HasKey(e => new { e.ApiApplicationId, e.BaseApiUserId });
                cfg.Property(e => e.ApiApplicationId).IsRequired().HasColumnOrder(0);
                cfg.Property(e => e.BaseApiUserId).IsRequired().HasColumnOrder(1);
                cfg.Property(e => e.RefreshToken).IsRequired()
                    .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute { IsUnique = true }));
                cfg.HasMany(e => e.ApiAuthenticationAccessTokens);
            });

            modelBuilder.Entity<ApiAuthenticationAccessToken>(
                cfg =>
                {
                    cfg.ToTable("AspNetExtApiAuthenticationAccessTokens").HasKey(e => e.Id);
                    cfg.Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity).IsRequired();
                    cfg.Property(e => e.ApiApplicationId).IsRequired();
                    cfg.Property(e => e.BaseApiUserId).IsRequired();
                    cfg.Property(e => e.AccessToken).IsRequired()
                        .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute { IsUnique = true }));
                    cfg.Property(e => e.ExpirationDate).IsRequired()
                        .HasColumnType("datetime2")
                        .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute()));
                });
        }

        /// <summary>
        ///     Validates that UserNames are unique and case insenstive
        /// </summary>
        /// <param name="entityEntry"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry,
            IDictionary<object, object> items)
        {
            if (entityEntry != null && entityEntry.State == EntityState.Added)
            {
                var errors = new List<DbValidationError>();
                var user = entityEntry.Entity as IdentityUser;
                //check for uniqueness of user name and email
                if (user != null)
                {
                    if (IdentityUsers.Any(u => String.Equals(u.UserName, user.UserName)))
                    {
                        errors.Add(new DbValidationError("User",
                            String.Format(CultureInfo.CurrentCulture, IdentityResources.DuplicateUserName, user.UserName)));
                    }
                    if (RequireUniqueEmail && IdentityUsers.Any(u => String.Equals(u.Email, user.Email)))
                    {
                        errors.Add(new DbValidationError("User",
                            String.Format(CultureInfo.CurrentCulture, IdentityResources.DuplicateEmail, user.Email)));
                    }
                }
                else
                {
                    var role = entityEntry.Entity as IdentityRole;
                    //check for uniqueness of role name
                    if (role != null && Roles.Any(r => String.Equals(r.Name, role.Name)))
                    {
                        errors.Add(new DbValidationError("Role",
                            String.Format(CultureInfo.CurrentCulture, IdentityResources.RoleAlreadyExists, role.Name)));
                    }
                }
                if (errors.Any())
                {
                    return new DbEntityValidationResult(entityEntry, errors);
                }
            }
            return base.ValidateEntity(entityEntry, items);
        }
    }

    /// <summary>
    /// Extension methods for code first configuration
    /// </summary>
    public static class EntityTypeConfigurationExtensions
    {
        /// <summary>
        /// Configures an entity using an action parameter
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="configurations"></param>
        /// <typeparam name="TEntityType"></typeparam>
        /// <returns></returns>
        public static EntityTypeConfiguration<TEntityType> Entity<TEntityType>(
            this DbModelBuilder modelBuilder, Action<EntityTypeConfiguration<TEntityType>> configurations)
            where TEntityType : class
        {
            var entityTypeConfiguration = modelBuilder.Entity<TEntityType>();
            configurations(entityTypeConfiguration);
            return entityTypeConfiguration;
        }
    }
}
