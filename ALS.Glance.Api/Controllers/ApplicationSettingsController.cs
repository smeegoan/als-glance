using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using ALS.Glance.Api.Helpers;
using ALS.Glance.Api.Helpers.ODataInterfaces;
using ALS.Glance.Api.Properties;
using ALS.Glance.Api.Security;
using ALS.Glance.Api.Security.Filters;
using ALS.Glance.Core.Security;
using ALS.Glance.Models;
using ALS.Glance.Models.Security.Implementations;
using ALS.Glance.UoW;
using ALS.Glance.UoW.Core;
using ALS.Glance.UoW.Core.Exceptions;
using Newtonsoft.Json;

namespace ALS.Glance.Api.Controllers
{
    public class ApplicationSettingsController : ODataController,
        ODataGet<ApplicationSettings>.WithKeyAndOptions<string, string>,
        ODataPost<ApplicationSettings>,
        ODataPut<ApplicationSettings>.WithKeyAndOptions<string, string>,
        ODataPatch<ApplicationSettings>.WithKey<string, string>,
        ODataDelete.WithKey<string, string>
    {
        private readonly IALSUnitOfWork _uow;

        public ApplicationSettingsController(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _uow = unitOfWorkFactory.Get<IALSUnitOfWork>();
        }

        #region ODataGet

        [EnableQuery, ApiAuthorize(Roles.Admin)]
        public IQueryable<ApplicationSettings> Get(ODataQueryOptions<ApplicationSettings> options)
        {
            return _uow.ApplicationSettings.GetAll();
        }

        [EnableQuery,
         EnableCors, ApiAuthorize(Roles.Admin, Roles.User)]
        public async Task<IHttpActionResult> Get(
            [FromODataUri] string userId, [FromODataUri] string applicationId,
            ODataQueryOptions<ApplicationSettings> options, CancellationToken ct)
        {
            return await Task.Run(() =>
            {
                var byIdQuery =
                    _uow.ApplicationSettings.GetAll().Where(c => c.ApplicationId == applicationId && c.UserId == userId);
                if (options.IfNoneMatch != null)
                {
                    var settingsQuery = options.IfNoneMatch.ApplyTo(byIdQuery);
                    return !settingsQuery.Any() ? StatusCode(HttpStatusCode.NotModified) : GetEntity(byIdQuery);
                    // The entity has a different ETag than the one specified in the If-None-Match header of the request,
                    // so we return the entity.
                }
                // The request didn't contain any ETag, so we return the entity.
                return GetEntity(byIdQuery);
            }, ct);
        }

        #endregion

        #region ODataPost

        [EnableCors, ApiAuthorize(Roles.Admin, Roles.User)]
        public async Task<IHttpActionResult> Post(ApplicationSettings entity, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return Request.CreateBadRequestResult(Resources.BadRequestErrorMessage, ModelState);


            var dbEntity =
                await _uow.ApplicationSettings.GetByUserIdAndApplicationIdAsync(
                    entity.UserId, entity.ApplicationId, ct);
            if (dbEntity != null)
            {
                //return Request.CreateConflictResponse("Duplicated"); 
                //Esoterica host does not allow the verbs DELETE, PUT or PATCH so we update the entity here

                return await Put(entity.UserId, entity.ApplicationId, entity, null, ct);
            }

            await _uow.BeginAsync(ct);
            entity.Application =
                await _uow.Security.Applications.GetByIdAsync(
                    entity.ApplicationId, ct);
            if (entity.Application == null)
                return NotFound();

            entity.CreatedOn = entity.UpdatedOn = DateTimeOffset.Now;
            if (entity.Values == null)
                entity.Values = new Dictionary<string, object>();
            entity.Value = JsonConvert.SerializeObject(entity.Values);
            entity = await _uow.ApplicationSettings.AddAsync(entity, ct);

            await _uow.CommitAsync(ct);

            return Created(entity);
        }

        #endregion

        #region ODataPut

        [ApiAuthorize(Roles.Admin, Roles.User), EnableCors, EnableQuery]
        public async Task<IHttpActionResult> Put(
            [FromODataUri] string userId, [FromODataUri] string applicationId, ApplicationSettings update, ODataQueryOptions<ApplicationSettings> options, CancellationToken ct)
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
                    await _uow.ApplicationSettings.GetByUserIdAndApplicationIdAsync(userId, applicationId, ct);
                if (options != null && options.IfMatch != null)
                {
                    if (entityToUpdate == null)
                    {
                        // The entity doesn't exist on the database and as the request contains an If-Match header we don't
                        // insert the entity instead (No UPSERT behavior if the If-Match header is present).
                        return NotFound();
                    }
                    if (!options.IfMatch.ApplyTo(_uow.ApplicationSettings.GetAll().Where(c => c.ApplicationId == applicationId && c.UserId == userId)).Any())
                    {
                        // The ETag of the entity doesn't match the value sent on the If-Match header, so the entity has
                        // been modified by a third party between the entity retrieval and update..
                        return StatusCode(HttpStatusCode.PreconditionFailed);
                    }
                    // The entity exists in the database and the ETag of the entity matches the value on the If-Match 
                    // header, so we update the entity.
                    entityToUpdate.UpdatedOn = DateTimeOffset.Now;
                    entityToUpdate.Values = update.Values ?? new Dictionary<string, object>();
                    entityToUpdate.Value = JsonConvert.SerializeObject(entityToUpdate.Values);
                    await _uow.CommitAsync(ct);
                    return Ok(update);
                }
                if (entityToUpdate == null)
                {
                    // The request didn't contain any If-Match header and the entity doesn't exist on the database, so
                    // we create a new one. For more details see the section 11.4.4 of the OData v4.0 specification.
                    update.UpdatedOn = update.CreatedOn = DateTimeOffset.Now;
                    update = await _uow.ApplicationSettings.AddAsync(update, ct);
                    await _uow.CommitAsync(ct);
                    return Created(update);
                }
                // the request didn't contain any If-Match header and the entity exists on the database, so we
                // update it's value.
                entityToUpdate.UpdatedOn = DateTimeOffset.Now;
                entityToUpdate.Values = update.Values ?? new Dictionary<string, object>();
                entityToUpdate.Value = JsonConvert.SerializeObject(entityToUpdate.Values);
                await _uow.CommitAsync(ct);
                return Ok(entityToUpdate);
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

        [ApiAuthorize(Roles.Admin, Roles.User),
        Permission(Role = Roles.User, ClaimType = ClaimTypes.Name, MustOwn = "UserId")]
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

                if (dbEntity.Values == null)
                    dbEntity.Values = new Dictionary<string, object>();
                dbEntity.Value = JsonConvert.SerializeObject(dbEntity.Values);
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

        [ApiAuthorize(Roles.Admin)]
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

        #region Private Methods

        private IHttpActionResult GetEntity(IQueryable<ApplicationSettings> byIdQuery)
        {
            var entity = byIdQuery.SingleOrDefault();
            if (entity == null)
                return NotFound();

            entity.Values = JsonConvert.DeserializeObject<Dictionary<string, object>>(entity.Value);
            return Ok(entity.ToSingleResult());
        }

        #endregion
    }
}