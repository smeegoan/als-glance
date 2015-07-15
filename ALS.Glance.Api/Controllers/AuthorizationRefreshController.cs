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
using ALS.Glance.Api.Security.Filters;
using ALS.Glance.Core.Security;
using ALS.Glance.Models.Security.Implementations;
using ALS.Glance.UoW;
using ALS.Glance.UoW.Core;

namespace ALS.Glance.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [AllowAnonymous]
    public class AuthorizationRefreshController : ODataController, 
        ODataGet<AuthorizationRefresh>.WithKey<string, string>, ODataPost<AuthorizationRefresh>
    {
        private readonly IALSUnitOfWork _uow;

        /// <summary>
        /// 
        /// </summary>
        public AuthorizationRefreshController(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _uow = unitOfWorkFactory.Get<IALSUnitOfWork>();
        }

        #region ODataGet<AuthorizationRefresh>.WithKey<string, string>

        [EnableQuery, ApiAuthorize(Roles.Admin)]
        public IQueryable<AuthorizationRefresh> Get()
        {
            throw new HttpResponseException(Request.CreateNotImplementedResponse());
        }

        [EnableQuery, AllowAnonymous]
        public async Task<IHttpActionResult> Get(
            [FromODataUri] string applicationId, [FromODataUri] string refreshToken, CancellationToken ct)
        {
            var applicationIdDecoded = applicationId.DecodeFromBase64ASCII();
            var refreshTokenDecoded = Guid.Parse(refreshToken.DecodeFromBase64ASCII());

            await _uow.BeginAsync(ct);

            var apiApplication = await _uow.Security.Applications.GetByIdAsync(applicationIdDecoded, ct);
            if (apiApplication == null)
            {
                ModelState.AddModelError(
                    "applicationId", Resources.Conflict_Authorizations_ApiApplicationNotFoundScoped);
                return Request.CreateConflictResponse(Resources.Conflict_Shared_GenericMessage, ModelState);
            }

            var authenticationToken =
                await _uow.ExecuteQueryAndGetFirstOrDefaultAsync(
                    _uow.Security.ApiAuthenticationTokens.GetAllAndFetch(e => e.BaseApiUser)
                        .Where(at =>
                            at.ApiApplicationId == applicationIdDecoded
                            && at.RefreshToken == refreshTokenDecoded), ct);

            if (authenticationToken == null)
            {
                ModelState.AddModelError(
                    "refreshToken", Resources.Conflict_Authorizations_RefreshTokenNotFound);
                return Request.CreateConflictResponse(Resources.Conflict_Shared_GenericMessage, ModelState);
            }

            //  Always create a new access token
            var authenticationAccessToken =
                new ApiAuthenticationAccessToken
                {
                    AccessToken = Guid.NewGuid(),
                    ExpirationDate = DateTime.Now.AddHours(1)
                };
            authenticationToken.ApiAuthenticationAccessTokens.Add(authenticationAccessToken);

            await _uow.CommitAsync(ct);

            var entity =
                new AuthorizationRefresh
                {
                    ApplicationId = applicationId,
                    RefreshToken = refreshToken,
                    Authorization =
                        new Authorization
                        {
                            AccessToken = authenticationAccessToken.AccessToken.ToString().EncodeToBase64ASCII(),
                            RefreshToken = authenticationToken.RefreshToken.ToString().EncodeToBase64ASCII(),
                            ExpiresIn =
                                (int) authenticationAccessToken.ExpirationDate.Subtract(DateTime.Now).TotalMinutes,
                            UserName = authenticationToken.BaseApiUser.UserName
                        }
                };

            return Ok(entity);
        }

        #endregion

        #region ODataPost<AuthorizationRefresh>

        [EnableQuery, AllowAnonymous]
        public async Task<IHttpActionResult> Post(AuthorizationRefresh entity, CancellationToken ct)
        {
            if (entity == null)
            {
                ModelState.AddModelError("entity", Resources.ModelValidation_RequiredScoped);
                return Request.CreateBadRequestResult(Resources.BadRequestErrorMessage, ModelState);
            }

            var applicationIdDecoded = entity.ApplicationId;
            var refreshTokenDecoded = Guid.Parse(entity.RefreshToken.DecodeFromBase64ASCII());

            await _uow.BeginAsync(ct);

            var apiApplication = await _uow.Security.Applications.GetByIdAsync(applicationIdDecoded, ct);
            if (apiApplication == null)
            {
                ModelState.AddModelError(
                    "request.ApplicationId", Resources.Conflict_Authorizations_ApiApplicationNotFoundScoped);
                return Request.CreateConflictResponse(Resources.Conflict_Shared_GenericMessage, ModelState);
            }

            var authenticationToken =
                await _uow.ExecuteQueryAndGetFirstOrDefaultAsync(
                    _uow.Security.ApiAuthenticationTokens.GetAllAndFetch(e => e.BaseApiUser)
                        .Where(at =>
                            at.ApiApplicationId == applicationIdDecoded
                            && at.RefreshToken == refreshTokenDecoded), ct);

            if (authenticationToken == null)
            {
                ModelState.AddModelError(
                    "request.RefreshToken", Resources.Conflict_Authorizations_RefreshTokenNotFound);
                return Request.CreateConflictResponse(Resources.Conflict_Shared_GenericMessage, ModelState);
            }

            //  Always create a new access token
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
                    AccessToken = authenticationAccessToken.AccessToken.ToString().EncodeToBase64ASCII(),
                    RefreshToken = authenticationToken.RefreshToken.ToString().EncodeToBase64ASCII(),
                    ExpiresIn = (int)authenticationAccessToken.ExpirationDate.Subtract(DateTime.Now).TotalMinutes,
                    UserName = authenticationToken.BaseApiUser.UserName
                };

            return Created(entity);
        }

        #endregion
    }
}