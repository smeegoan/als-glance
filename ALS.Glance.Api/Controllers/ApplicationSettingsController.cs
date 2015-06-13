using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using ALS.Glance.Api.Helpers;
using ALS.Glance.Api.Properties;
using ALS.Glance.Api.Security;
using ALS.Glance.Api.Security.Filters;
using ALS.Glance.Models;
using ALS.Glance.UoW;
using ALS.Glance.UoW.Core;
using ALS.Glance.UoW.Core.Exceptions;
using Newtonsoft.Json;

namespace ALS.Glance.Api.Controllers
{
    //  [Authorize]
    public class ApplicationSettingsController : ODataController, ODataGet<ApplicationSettings>.WithKey<string, string>,
        ODataPost<ApplicationSettings>, ODataPut<ApplicationSettings>.WithKey<string, string>, ODataPatch<ApplicationSettings>.WithKey<string, string>, ODataDelete.WithKey<string, string>
    {
        private readonly IALSUnitOfWork _uow;

        public ApplicationSettingsController(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _uow = unitOfWorkFactory.Get<IALSUnitOfWork>();
        }

        #region ODataGet

        [EnableQuery
            //, ApiAuthorize(ServiceRoles.Admin)
        ]
        public IQueryable<ApplicationSettings> Get()
        {
            return _uow.ApplicationSettings.GetAll();
        }

        [EnableQuery,
        CorsPolicy,
            //ApiAuthorize(ServiceRoles.Admin, ServiceRoles.User),
            //Permission(Role = ServiceRoles.User, ClaimType = ClaimTypes.Name, MustOwn = "UserId")
        ]
        public async Task<IHttpActionResult> Get(
            [FromODataUri] string userId, [FromODataUri] string applicationId, CancellationToken ct)
        {
            var entity =
                await _uow.ApplicationSettings.GetByUserIdAndApplicationIdAsync(
                    userId, applicationId, ct);
            if (entity == null)
                return (IHttpActionResult)NotFound();

            return Ok(entity.ToSingleResult());
        }

        #endregion

        #region ODataPost

        //        [ApiAuthorize(ServiceRoles.Admin, ServiceRoles.User)]
        [CorsPolicy]
        public async Task<IHttpActionResult> Post(ApplicationSettings entity, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return Request.CreateBadRequestResult(Resources.BadRequestErrorMessage, ModelState);

            await _uow.BeginAsync(ct);

            var dbEntity =
                await _uow.ApplicationSettings.GetByUserIdAndApplicationIdAsync(
                    entity.UserId, entity.ApplicationId, ct);
            if (dbEntity != null)
                return Request.CreateConflictResponse("Duplicated");

            entity.Application =
                await _uow.Security.ApiApplications.GetByIdAsync(
                    entity.ApplicationId, ct);
            if (entity.Application == null)
                return NotFound();

            entity.User =
                await _uow.Security.ApiUsers.GetByIdAsync(
                    entity.UserId, ct);
            if (entity.User == null)
                return NotFound();

            entity.CreatedOn = entity.UpdatedOn = DateTimeOffset.Now;
            entity = await _uow.ApplicationSettings.AddAsync(entity, ct);

            await _uow.CommitAsync(ct);

            return Created(entity);
        }

        #endregion

        #region ODataPut

        //[ApiAuthorize(ServiceRoles.Admin, ServiceRoles.User),
        //Permission(Role = ServiceRoles.User, ClaimType = ClaimTypes.Name, MustOwn = "UserId")]
        [CorsPolicy,
        EnableQuery]
        public async Task<IHttpActionResult> Put(
            [FromODataUri] string userId, [FromODataUri] string applicationId, ApplicationSettings update, CancellationToken ct)
        {
            if (update == null)
            {
                ModelState.AddModelError("entity", Resources.ModelValidation_RequiredScoped);
                return Request.CreateBadRequestResult(Resources.BadRequestErrorMessage, ModelState);
            }

            if (userId != update.UserId)
                ModelState.AddModelError("entity.UserId", "Does not match with URL key");
            if (applicationId != update.ApplicationId)
                ModelState.AddModelError("entity.ApplicationId", "Does not match with URL key");
            if (!ModelState.IsValid)
                return Request.CreateBadRequestResult(Resources.BadRequestErrorMessage, ModelState);

            try
            {
                await _uow.BeginAsync(ct);

                var entityToUpdate =
                    await _uow.ApplicationSettings.GetByUserIdAndApplicationIdAsync(
                        userId, applicationId, ct);
                if (entityToUpdate == null)
                    return NotFound();

                entityToUpdate.UpdatedOn = DateTimeOffset.Now;
                entityToUpdate.Value = update.Value;
                entityToUpdate = await _uow.ApplicationSettings.UpdateAsync(entityToUpdate, ct);

                await _uow.CommitAsync(ct);

                return Updated(entityToUpdate);
            }
            catch (ConcurrencyException)
            {
                if (_uow.ApplicationSettings
                    .GetByUserIdAndApplicationId(userId, applicationId) == null)
                    return NotFound();
                throw;
            }
        }

        #endregion

        #region ODataPatch

        [ApiAuthorize(ServiceRoles.Admin, ServiceRoles.User),
        Permission(Role = ServiceRoles.User, ClaimType = ClaimTypes.Name, MustOwn = "UserId")]
        public async Task<IHttpActionResult> Patch(
            [FromODataUri] string userId, [FromODataUri] string applicationId, Delta<ApplicationSettings> entity, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return Request.CreateBadRequestResult(Resources.BadRequestErrorMessage, ModelState);

            try
            {
                await _uow.BeginAsync(ct);

                var dbEntity =
                    await _uow.ApplicationSettings.GetByUserIdAndApplicationIdAsync(
                        userId, applicationId, ct);
                if (dbEntity == null)
                    return NotFound();

                entity.Patch(dbEntity);
                dbEntity.UpdatedOn = DateTimeOffset.Now;
                dbEntity = await _uow.ApplicationSettings.UpdateAsync(dbEntity, ct);
                await _uow.CommitAsync(ct);

                return Updated(dbEntity);
            }
            catch (ConcurrencyException)
            {
                if (_uow.ApplicationSettings
                    .GetByUserIdAndApplicationId(userId, applicationId) == null)
                    return NotFound();
                throw;
            }
        }

        #endregion

        #region ODataDelete

#warning Desactivar delete dos utilizadores normais

        [ApiAuthorize(ServiceRoles.Admin, ServiceRoles.User)]
        public async Task<IHttpActionResult> Delete(
            [FromODataUri] string userId, [FromODataUri] string applicationId, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return Request.CreateBadRequestResult(Resources.BadRequestErrorMessage, ModelState);

            await _uow.BeginAsync(ct);

            var entity =
                await _uow.ApplicationSettings.GetByUserIdAndApplicationIdAsync(
                    userId, applicationId, ct);
            if (entity == null)
                return (IHttpActionResult)NotFound();
            await _uow.ApplicationSettings.DeleteAsync(entity, ct);

            await _uow.CommitAsync(ct);

            return Request.CreateDeletedResponse(null);
        }

        #endregion
    }
}