using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using ALS.Glance.Api.Helpers;
using ALS.Glance.Api.Models;
using ALS.Glance.Api.Properties;
using ALS.Glance.Api.Security;
using ALS.Glance.Api.Security.Extensions;
using ALS.Glance.Api.Security.Filters;
using ALS.Glance.Core.Security;
using ALS.Glance.Models.Security.Implementations;
using ALS.Glance.UoW;
using ALS.Glance.UoW.Core;
using Microsoft.AspNet.Identity;

namespace ALS.Glance.Api.Controllers
{

    /// <summary>
    /// 
    /// </summary>
    public class ApiUserController : ODataController, ODataGet<IdentityUser>.WithKey<string>
    {
        private readonly IUserTokenProvider<IdentityUser, string> _userTokenProvider;
        private readonly IIdentityMessageService _emailService;
        private readonly IALSUnitOfWork _uow;

        public ApiUserController(IUnitOfWorkFactory unitOfWorkFactory, IUserTokenProvider<IdentityUser, string> userTokenProvider,
            IIdentityMessageService emailService)
        {
            _userTokenProvider = userTokenProvider;
            _emailService = emailService;
            _uow = unitOfWorkFactory.Get<IALSUnitOfWork>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [EnableQuery, ApiAuthorize(Roles.Admin)]
        public IQueryable<IdentityUser> Get()
        {
            return _uow.Security.BaseIdentities.GetAll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [EnableQuery,
        ApiAuthorize(Roles.Admin, Roles.User),
        Permission(Role = Roles.User, ClaimType = ClaimTypes.Name, MustOwn = "key")]
        public async Task<IHttpActionResult> Get([FromODataUri] string key, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var user = await _uow.Security.GetUserManager<IdentityUser>().FindByNameAsync(key);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [ApiAuthorize(Roles.Admin, Roles.User),
        Permission(Role = Roles.User, ClaimType = ClaimTypes.Name, MustOwn = "key")]
        public async Task<IHttpActionResult> ChangePassword([FromODataUri] string key, ODataActionParameters parameters, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return Request.CreateBadRequestResult(Resources.BadRequestErrorMessage, ModelState);

            var newPassword = (string)parameters[FieldNames.ApiUser_NewPassword];
            var currentPassword = (string)parameters[FieldNames.ApiUser_CurrentPassword];
            await _uow.BeginAsync(ct);

            ct.ThrowIfCancellationRequested();
            var userManager = _uow.Security.GetUserManager<IdentityUser>();
            var user = await userManager.FindByNameAsync(key);
            if (user == null)
                return NotFound();

            var result = await userManager.ChangePasswordAsync(user.Id, currentPassword, newPassword);
            result.ThrowIfFailed(Resources.Conflict_Users_IncorrectOldPassword);

            await _uow.CommitAsync(ct);

            return Ok();
        }

        public async Task<IHttpActionResult> ResetPassword([FromODataUri] string key, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return Request.CreateBadRequestResult(Resources.BadRequestErrorMessage, ModelState);

            ct.ThrowIfCancellationRequested();

            var userManager = GetConfiguredApiUserManager(_uow);
            var user = await userManager.FindByNameAsync(key);
            if (user == null)
                return Ok();

            await SendPasswordResetConfirmationEmailAsync(user, userManager, ct);

            return Ok();
        }

        #region Private Methods

        private UserManager<IdentityUser> GetConfiguredApiUserManager(IALSUnitOfWork uow)
        {
            var userManager = uow.Security.GetUserManager<IdentityUser>();
            ((UserValidator<IdentityUser, string>)userManager.UserValidator)
                .AllowOnlyAlphanumericUserNames = false;
            userManager.UserTokenProvider = _userTokenProvider;
            userManager.EmailService = _emailService;

            return userManager;
        }

        private async Task SendPasswordResetConfirmationEmailAsync<TApiUser>(
            TApiUser user, UserManager<TApiUser> userManager, CancellationToken ct)
            where TApiUser : IdentityUser
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user.Id);
            token = token.EncodeToBase64ASCII();

            var userId = user.Id.EncodeToBase64ASCII();

            var emailConfirmationUrl =
                Url.Link("Default",
                    new
                    {
                        Controller = "Account",
                        Action = "PasswordResetConfirmation",
                        t = token,
                        e = userId
                    });

            await userManager.SendEmailPasswordResetConfirmationAsync(user.Id, emailConfirmationUrl, ct);
        }

        #endregion
    }
}