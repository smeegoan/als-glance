namespace ALS.Glance.UoW.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Security : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        AccessFailedCount = c.Int(nullable: false),
                        Email = c.String(),
                        EmailConfirmed = c.Boolean(nullable: false),
                        LockoutEnabled = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        SecurityStamp = c.String(),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        UserName = c.String(nullable: false, maxLength: 256),
                        Description = c.String(maxLength: 512),
                        CreatedOn = c.DateTimeOffset(nullable: false, precision: 7),
                        CreatedBy = c.String(),
                        UpdatedOn = c.DateTimeOffset(nullable: false, precision: 7),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetExtApiAuthenticationTokens",
                c => new
                    {
                        ApiApplicationId = c.String(nullable: false, maxLength: 128),
                        BaseApiUserId = c.String(nullable: false, maxLength: 128),
                        RefreshToken = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApiApplicationId, t.BaseApiUserId })
                .ForeignKey("dbo.AspNetExtApiApplications", t => t.ApiApplicationId)
                .ForeignKey("dbo.AspNetExtApiUsers", t => t.BaseApiUserId)
                .Index(t => t.ApiApplicationId)
                .Index(t => t.BaseApiUserId)
                .Index(t => t.RefreshToken, unique: true);
            
            CreateTable(
                "dbo.AspNetExtApiAuthenticationAccessTokens",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ApiApplicationId = c.String(nullable: false, maxLength: 128),
                        BaseApiUserId = c.String(nullable: false, maxLength: 128),
                        AccessToken = c.Guid(nullable: false),
                        ExpirationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetExtApiAuthenticationTokens", t => new { t.ApiApplicationId, t.BaseApiUserId }, cascadeDelete: true)
                .Index(t => new { t.ApiApplicationId, t.BaseApiUserId })
                .Index(t => t.AccessToken, unique: true)
                .Index(t => t.ExpirationDate);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.ApplicationSettings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ApplicationId = c.String(nullable: false, maxLength: 128),
                        Value = c.String(maxLength: 4000),
                        CreatedOn = c.DateTimeOffset(nullable: false, precision: 7),
                        UpdatedOn = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetExtApiApplications", t => t.ApplicationId)
                .ForeignKey("dbo.AspNetExtApiUsers", t => t.UserId)
                .Index(t => new { t.UserId, t.ApplicationId }, unique: true, name: "IX_dbo.ApplicationSettings_UserIdApplicationId");
            
            CreateTable(
                "dbo.AspNetExtApplications",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.AspNetExtApiApplications",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetExtApplications", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.AspNetExtApiUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        GivenName = c.String(maxLength: 256),
                        FamilyName = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.AspNetExtSiteUser",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(maxLength: 256),
                        LastName = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.AspNetExtApiRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Description = c.String(maxLength: 512),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetRoles", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetExtApiRoles", "Id", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetExtSiteUser", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetExtApiUsers", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetExtApiApplications", "Id", "dbo.AspNetExtApplications");
            DropForeignKey("dbo.AspNetExtApplications", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationSettings", "UserId", "dbo.AspNetExtApiUsers");
            DropForeignKey("dbo.ApplicationSettings", "ApplicationId", "dbo.AspNetExtApiApplications");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetExtApiAuthenticationTokens", "BaseApiUserId", "dbo.AspNetExtApiUsers");
            DropForeignKey("dbo.AspNetExtApiAuthenticationAccessTokens", new[] { "ApiApplicationId", "BaseApiUserId" }, "dbo.AspNetExtApiAuthenticationTokens");
            DropForeignKey("dbo.AspNetExtApiAuthenticationTokens", "ApiApplicationId", "dbo.AspNetExtApiApplications");
            DropIndex("dbo.AspNetExtApiRoles", new[] { "Id" });
            DropIndex("dbo.AspNetExtSiteUser", new[] { "Id" });
            DropIndex("dbo.AspNetExtApiUsers", new[] { "Id" });
            DropIndex("dbo.AspNetExtApiApplications", new[] { "Id" });
            DropIndex("dbo.AspNetExtApplications", new[] { "Id" });
            DropIndex("dbo.ApplicationSettings", "IX_dbo.ApplicationSettings_UserIdApplicationId");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetExtApiAuthenticationAccessTokens", new[] { "ExpirationDate" });
            DropIndex("dbo.AspNetExtApiAuthenticationAccessTokens", new[] { "AccessToken" });
            DropIndex("dbo.AspNetExtApiAuthenticationAccessTokens", new[] { "ApiApplicationId", "BaseApiUserId" });
            DropIndex("dbo.AspNetExtApiAuthenticationTokens", new[] { "RefreshToken" });
            DropIndex("dbo.AspNetExtApiAuthenticationTokens", new[] { "BaseApiUserId" });
            DropIndex("dbo.AspNetExtApiAuthenticationTokens", new[] { "ApiApplicationId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropTable("dbo.AspNetExtApiRoles");
            DropTable("dbo.AspNetExtSiteUser");
            DropTable("dbo.AspNetExtApiUsers");
            DropTable("dbo.AspNetExtApiApplications");
            DropTable("dbo.AspNetExtApplications");
            DropTable("dbo.ApplicationSettings");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetExtApiAuthenticationAccessTokens");
            DropTable("dbo.AspNetExtApiAuthenticationTokens");
            DropTable("dbo.AspNetUsers");
        }
    }
}
