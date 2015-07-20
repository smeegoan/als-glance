using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using ALS.Glance.Api.Helpers;
using ALS.Glance.Api.Helpers.ODataInterfaces;
using ALS.Glance.Api.Models;
using ALS.Glance.Api.Properties;
using ALS.Glance.Api.Security;
using ALS.Glance.Api.Security.Extensions;
using ALS.Glance.Models.Security;
using ALS.Glance.Models.Security.Implementations;
using ALS.Glance.UoW;
using ALS.Glance.UoW.Core;
using Microsoft.AspNet.Identity;

namespace ALS.Glance.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [AllowAnonymous]
    public class AuthorizationRequestController : ODataController, ODataPost<AuthorizationRequest>
    {
        private readonly IALSUnitOfWork _uow;

        /// <summary>
        /// 
        /// </summary>
        public AuthorizationRequestController(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _uow = unitOfWorkFactory.Get<IALSUnitOfWork>();
        }

        #region ODataPost<AuthorizationRequest>

        [EnableQuery, AllowAnonymous]
        public async Task<IHttpActionResult> Post(AuthorizationRequest entity, CancellationToken ct)
        {
            if (entity == null)
            {
                ModelState.AddModelError("entity", Resources.ModelValidation_RequiredScoped);
                return Request.CreateBadRequestResult(Resources.BadRequestErrorMessage, ModelState);
            }


            await _uow.BeginAsync(ct);

            var user = await GetUserAsync(entity.UserName.Trim().ToLowerInvariant(), entity.Password, ct);
            if (user == null)
                return Request.CreateConflictResponse(Resources.Conflict_Authorizations_InvalidCredentials);
            if (!user.EmailConfirmed)
                return Request.CreateConflictResponse(Resources.Conflict_Authorizations_MissingEmailConfirmation);

            //  Try to find an existing token for the application
            var authenticationToken = await GetApiAuthenticationTokenAsync(user.Id, entity.ApplicationId, ct);
            if (authenticationToken == null)
            {
                //  Searches if the application exists
                var application = await GetApiApplicationUserAsync(entity.ApplicationId, ct);
                if (application == null)
                {
                    ModelState.AddModelError(
                        "request.ApplicationId", Resources.Conflict_Authorizations_ApiApplicationNotFoundScoped);
                    return Request.CreateConflictResponse(Resources.Conflict_Shared_GenericMessage, ModelState);
                }

                //  Create a new global refresh token without any access tokens
                authenticationToken =
                    await AddApiAuthenticationTokenAsync(user, application, ct);
            }
            //  Always create a new access token with a TTL of 1h
            var authenticationAccessToken =
                new ApiAuthenticationAccessToken
                {
                    AccessToken = Guid.NewGuid(),
                    ExpirationDate = DateTime.Now.AddHours(1)
                };
            authenticationToken.ApiAuthenticationAccessTokens.Add(authenticationAccessToken);

            await _uow.CommitAsync(ct);

            entity.Authorization =
                new Authorization
                {
                    AccessToken =
                        authenticationAccessToken.AccessToken.ToString()
                        .EncodeToBase64ASCII(),
                    RefreshToken =
                        authenticationToken.RefreshToken.ToString().EncodeToBase64ASCII(),
                    ExpiresIn =
                        (int)
                        authenticationAccessToken.ExpirationDate.Subtract(DateTime.Now)
                        .TotalMinutes,
                    UserName = user.UserName
                };

            return Created(entity);

        }

        #endregion

        #region Private Methods

        private async Task<ApiAuthenticationToken> GetApiAuthenticationTokenAsync(
            string userId, string applicationId, CancellationToken ct)
        {
            if (userId == null) throw new ArgumentNullException("userId");
            if (applicationId == null) throw new ArgumentNullException("applicationId");

            return
                await _uow.ExecuteQueryAndGetFirstOrDefaultAsync(
                    _uow.Security.ApiAuthenticationTokens.GetAll()
                        .Where(at =>
                            at.BaseApiUserId == userId
                            && at.ApiApplicationId == applicationId), ct);
        }

        private async Task<ApiUser> GetUserAsync(
            string userName, string password, CancellationToken ct)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (password == null) throw new ArgumentNullException("password");

            var userManager = _uow.Security.GetUserManager<ApiUser>();
            ((UserValidator<ApiUser, string>)userManager.UserValidator).AllowOnlyAlphanumericUserNames = false;


            //  Gets the user by username and password
            ct.ThrowIfCancellationRequested();
            var user =
                await userManager.FindAsync(userName.Trim().ToLowerInvariant(), password);

            return user;
        }

        private async Task<ApplicationUser> GetApiApplicationUserAsync(
            string applicationId, CancellationToken ct)
        {
            if (applicationId == null) throw new ArgumentNullException("applicationId");

            return await _uow.Security.Applications.GetByIdAsync(applicationId, ct);
        }

        private async Task<ApiAuthenticationToken> AddApiAuthenticationTokenAsync(
            ApiUser user, ApplicationUser application, CancellationToken ct)
        {
            return
                await _uow.Security.ApiAuthenticationTokens.AddAsync(
                        new ApiAuthenticationToken
                        {
                            ApiApplication = application,
                            BaseApiUser = user,
                            RefreshToken = Guid.NewGuid()
                        }, ct);
        }

        #endregion
    }
}
