using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using ALS.Glance.Api.Security.Exceptions;
using ALS.Glance.Api.Security.Extensions;
using ALS.Glance.Models.Security.Implementations;
using ALS.Glance.UoW.Security;
using ALS.Glance.UoW.Security.Context.Implementation;
using Microsoft.AspNet.Identity;

namespace ALS.Glance.Api.Security.Filters
{
    public class IdentityBearerAuthenticationAttribute : BearerAuthenticationAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorizationParameter"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="InvalidTokenFormatException"></exception>
        /// <exception cref="TokenNotFoundException"></exception>
        /// <exception cref="ApiAuthorizationExpiredException"></exception>
        protected async override Task<IPrincipal> AuthenticateAsync(string authorizationParameter, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            Guid accessToken;
            try
            {
                accessToken = Guid.Parse(authorizationParameter.DecodeFromBase64ASCII());
            }
            catch (Exception e)
            {
                throw new InvalidTokenFormatException(e);
            }

            ClaimsIdentity identity;
            using (var context = new SecurityDbContext("ALSContext"))
            using (var userStore = new SecurityUserStore<ApiUser>(context))
            using (var userManager = new UserManager<ApiUser>(userStore))
            {
                var authenticationToken =
                    await context.ApiAuthenticationAccessToken.Include(e => e.ApiAuthenticationToken.BaseApiUser)
                        .SingleOrDefaultAsync(a => a.AccessToken == accessToken, ct);

                if (authenticationToken == null)
                    throw new TokenNotFoundException();
                if (authenticationToken.ExpirationDate < DateTime.Now)
                    throw new ApiAuthorizationExpiredException();

                // Create a ClaimsIdentity with all the claims for this user.
                ct.ThrowIfCancellationRequested(); // Unfortunately, IClaimsIdenityFactory doesn't support CancellationTokens.
                identity = await
                    userManager.ClaimsIdentityFactory
                        .CreateAsync(
                            userManager, authenticationToken.ApiAuthenticationToken.BaseApiUser, SchemeId);

            }

            #region Cleanup

            if (DateTime.Now.Millisecond % 10 == 0)
                Task.Factory.StartNew(
                    () =>
                    {
                        using (var ctx = new SecurityDbContext("ALSContext"))
                        {
                            while (true)
                            {
                                var currentTime = DateTime.Now;

                                ctx.ApiAuthenticationAccessToken.RemoveRange(
                                    ctx.ApiAuthenticationAccessToken.Where(t => t.ExpirationDate < currentTime));
                                try
                                {
                                    ctx.SaveChanges();
                                    return;
                                }
                                catch (DbUpdateConcurrencyException e)
                                {
                                    e.Entries.Single().Reload();
                                }
                            }
                        }
                    }, ct);

            #endregion

            return new ClaimsPrincipal(identity);
        }
    }
}