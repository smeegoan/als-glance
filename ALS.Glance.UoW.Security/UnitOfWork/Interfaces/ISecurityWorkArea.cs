﻿using ALS.Glance.Models.Security.Implementations;
using ALS.Glance.Models.Security.Interfaces;
using ALS.Glance.UoW.Core;
using ALS.Glance.UoW.Security.Repositories.Interfaces;
using Microsoft.AspNet.Identity;

namespace ALS.Glance.UoW.Security.UnitOfWork.Interfaces
{
    public interface ISecurityWorkArea : IWorkArea
    {
        bool RequiresUniqueEmail { get; }
        UserManager<TUser> GetUserManager<TUser>(bool autoSaveChanges = true)
            where TUser : class, IIdentityUser<string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>, IUser;
        IApiAuthenticationAccessTokenRepository ApiAuthenticationAccessTokens { get; }
        IApiAuthenticationTokenRepository ApiAuthenticationTokens { get; }
        IApplicationRepository Applications { get; }
        IBaseIdentityRepository BaseIdentities { get; }
    }
}
