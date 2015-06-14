using System.Linq;
using ALS.Glance.Models.Security.Implementations;
using ALS.Glance.Models.Security.Interfaces;
using ALS.Glance.UoW.Security.Context.Implementation;
using ALS.Glance.UoW.Security.Repositories.Implementation;
using ALS.Glance.UoW.Security.Repositories.Interfaces;
using ALS.Glance.UoW.Security.UnitOfWork.Interfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Concurrent;

namespace ALS.Glance.UoW.Security.UnitOfWork.Implementation
{
    public class SecurityWorkArea : ISecurityWorkArea, IDisposable
    {
        private readonly SecurityDbContext _context;
        private readonly Lazy<IApiAuthenticationAccessTokenRepository> _lazyApiAuthenticationAccessTokeyRepository;
        private readonly Lazy<IApiAuthenticationTokenRepository> _lazyApiAuthenticationTokenRepository;
        private readonly Lazy<IApplicationRepository> _lazyApplicationRepository;
        private readonly Lazy<IBaseIdentityRepository> _lazyBaseIdentityRepository;

        private readonly ConcurrentDictionary<Type, IDisposable> _userManagersForDispose =
            new ConcurrentDictionary<Type, IDisposable>();

        public bool RequiresUniqueEmail
        {
            get { return _context.RequireUniqueEmail; }
        }

        public UserManager<TUser> GetUserManager<TUser>(bool autoSaveChanges = true) where TUser : class, IIdentityUser<string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>, IUser
        {
            var securityUserStore = new SecurityUserStore<TUser>(_context) { AutoSaveChanges = autoSaveChanges };

            var userManager =
                (UserManager<TUser>)
                    _userManagersForDispose.GetOrAdd(
                        typeof(TUser),
                        type => new UserManager<TUser>(securityUserStore));

            return userManager;
        }

        public IApiAuthenticationAccessTokenRepository ApiAuthenticationAccessTokens
        {
            get
            {
                return _lazyApiAuthenticationAccessTokeyRepository.Value;
            }
        }

        public IApiAuthenticationTokenRepository ApiAuthenticationTokens
        {
            get { return _lazyApiAuthenticationTokenRepository.Value; }
        }

        public IApplicationRepository Applications
        {
            get { return _lazyApplicationRepository.Value; }
        }

        public IBaseIdentityRepository BaseIdentities
        {
            get { return _lazyBaseIdentityRepository.Value; }
        }


        public SecurityWorkArea(SecurityDbContext context)
        {
            _context = context;
            _lazyApiAuthenticationAccessTokeyRepository = new Lazy<IApiAuthenticationAccessTokenRepository>(() => new ApiAuthenticationAccessTokenRepository(context));
            _lazyApiAuthenticationTokenRepository = new Lazy<IApiAuthenticationTokenRepository>(() => new ApiAuthenticationTokenRepository(context));
            _lazyApplicationRepository = new Lazy<IApplicationRepository>(() => new ApplicationRepository(context));
            _lazyBaseIdentityRepository = new Lazy<IBaseIdentityRepository>(() => new BaseIdentityRepository(context));
        }

        ~SecurityWorkArea()
        {
            Dispose(false);
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            foreach (var disposable in _userManagersForDispose.Values.ToArray())
                disposable.Dispose();
            _userManagersForDispose.Clear();
        }

        #endregion
    }
}
