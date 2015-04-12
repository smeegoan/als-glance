using System;
using ALS.Glance.Api.Security;
using ALS.Glance.Models;
using ALS.Glance.UoW;
using ALS.Glance.UoW.Core;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using ALS.Glance.UoW.Core.Exceptions;

namespace ALS.Glance.Api.Controllers
{
    public class FactController : ODataController,
        ODataGet<Fact>.WithKey<long>,
        ODataPost<Fact>,
        ODataPut<Fact>.WithKey<long>,
        ODataPatch<Fact>.WithKey<long>,
        ODataDelete.WithKey<long>
    {
        private readonly IALSUnitOfWork _uow;

        public FactController(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _uow = unitOfWorkFactory.Get<IALSUnitOfWork>();
        }

        #region ODataGet

        [EnableQuery, CorsPolicy]
        public IQueryable<Fact> Get()
        {
            return _uow.Facts.GetAll();
        }

        [EnableQuery]
        public async Task<IHttpActionResult> Get([FromODataUri] long key, CancellationToken ct)
        {
            var entity = await _uow.Facts.GetByIdAsync(key, ct);
            if (entity == null)
                return NotFound();
            return Ok(SingleResult.Create(new[] { entity }.AsQueryable()));
        }

        #endregion

        #region ODataPost

        public async Task<IHttpActionResult> Post(Fact entity, CancellationToken ct)
        {
            if (entity == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            await _uow.BeginAsync(ct);
            entity.CreatedOn = DateTimeOffset.Now;
            entity.UpdatedOn = DateTimeOffset.Now;
            entity = await _uow.Facts.AddAsync(entity, ct);
            await _uow.CommitAsync(ct);
            return Created(entity);
        }

        #endregion

        #region ODataPut

        public async Task<IHttpActionResult> Put([FromODataUri] long key, Fact update, CancellationToken ct)
        {
            if (update == null || !ModelState.IsValid)
                return BadRequest(ModelState);
            if (key != update.Id)
                return BadRequest();

            try
            {
                await _uow.BeginAsync(ct);
                update = await _uow.Facts.UpdateAsync(update, ct);
                await _uow.CommitAsync(ct);

                return Updated(update);
            }
            catch (ConcurrencyException)
            {
                if (_uow.Facts.GetById(key) == null)
                    return NotFound();
                throw;
            }
        }

        #endregion

        #region ODataPatch

        public async Task<IHttpActionResult> Patch([FromODataUri] long key, Delta<Fact> entity, CancellationToken ct)
        {
            if (entity == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _uow.BeginAsync(ct);

                var dbEntity = await _uow.Facts.GetByIdAsync(key, ct);
                if (dbEntity == null)
                    return NotFound();

                entity.Patch(dbEntity);
                dbEntity = await _uow.Facts.UpdateAsync(dbEntity, ct);
                await _uow.CommitAsync(ct);

                return Updated(dbEntity);
            }
            catch (ConcurrencyException)
            {
                if (_uow.Facts.GetById(key) == null)
                    return NotFound();
                throw;
            }
        }

        #endregion

        #region ODataDelete

        public async Task<IHttpActionResult> Delete([FromODataUri] long key, CancellationToken ct)
        {
            await _uow.BeginAsync(ct);
            var entity = await _uow.Facts.GetByIdAsync(key, ct);
            if (entity == null)
                return NotFound();

            await _uow.Facts.DeleteAsync(entity, ct);
            await _uow.CommitAsync(ct);
            return StatusCode(HttpStatusCode.NoContent);
        }

        #endregion
    }
}