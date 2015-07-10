﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using ALS.Glance.Api.Helpers;
using ALS.Glance.Api.Helpers.ODataInterfaces;
using ALS.Glance.Api.Properties;
using ALS.Glance.Api.Security;
using ALS.Glance.Api.Security.Filters;
using ALS.Glance.Models;
using ALS.Glance.Models.Security.Implementations;
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

        [EnableQuery, ApiAuthorize(Roles.Admin)
        ]
        public IQueryable<ApplicationSettings> Get()
        {
            return _uow.ApplicationSettings.GetAll();
        }

        [EnableQuery,
        EnableCors, ApiAuthorize(Roles.Admin, Roles.Application, Roles.User), 
        Permission(Role = Roles.User, ClaimType = ClaimTypes.Name, MustOwn = "UserId")]
        public async Task<IHttpActionResult> Get(
            [FromODataUri] string userId, [FromODataUri] string applicationId, CancellationToken ct)
        {
            var entity =
                await _uow.ApplicationSettings.GetByUserIdAndApplicationIdAsync(
                    userId, applicationId, ct);
            if (entity == null)
                return (IHttpActionResult)NotFound();

            entity.Values = JsonConvert.DeserializeObject<Dictionary<string, object>>(entity.Value);
            return Ok(entity.ToSingleResult());
        }

        #endregion

        #region ODataPost

        [EnableCors, ApiAuthorize(Roles.Admin, Roles.Application, Roles.User)]
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

                return await Put(entity.UserId, entity.ApplicationId, entity, ct);
            }

            await _uow.BeginAsync(ct);
            entity.Application =
                await _uow.Security.Applications.GetByIdAsync(
                    entity.ApplicationId, ct);
            if (entity.Application == null)
                return NotFound();

            entity.User = await _uow.Security.GetUserManager<IdentityUser>().FindByIdAsync(entity.UserId);
            if (entity.User == null)
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

        [ApiAuthorize(Roles.Admin, Roles.User),
        Permission(Role =Roles.User, ClaimType = ClaimTypes.Name, MustOwn = "UserId"),
        EnableCors,
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
                {
                    update.UpdatedOn = update.CreatedOn = DateTimeOffset.Now;
                    update = await _uow.ApplicationSettings.AddAsync(update, ct);
                }
                else
                {
                    entityToUpdate.UpdatedOn = DateTimeOffset.Now;
                    entityToUpdate.Values = update.Values ?? new Dictionary<string, object>();
                    entityToUpdate.Value = JsonConvert.SerializeObject(entityToUpdate.Values);
                }

                await _uow.CommitAsync(ct);

                return Updated(entityToUpdate ?? update);
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
    }
}