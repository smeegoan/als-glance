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
using ALS.Glance.Core.Security;
using ALS.Glance.Models.Security.Implementations;
using ALS.Glance.UoW;
using ALS.Glance.UoW.Core;
using log4net;
using Microsoft.AspNet.Identity;

namespace ALS.Glance.Api.Controllers
{
    [ AllowAnonymous]
    public class ApiUserRegistrationController : ODataController, ODataPost<ApiUserRegistration>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ApiUserRegistrationController));

        private readonly IUserTokenProvider<ApiUser, string> _userTokenProvider;
        private readonly IIdentityMessageService _emailService;
        private readonly IALSUnitOfWork _uow;

        public ApiUserRegistrationController(
            IUnitOfWorkFactory unitOfWorkFactory,
            IUserTokenProvider<ApiUser, string> userTokenProvider,
            IIdentityMessageService emailService)
        {
            _userTokenProvider = userTokenProvider;
            _emailService = emailService;
            _uow = unitOfWorkFactory.Get<IALSUnitOfWork>();
        }


        #region ODataPost<ApiUserRegistration>

        [EnableQuery]
        public async Task<IHttpActionResult> Post(ApiUserRegistration entity, CancellationToken ct)
        {
            if (entity == null)
            {
                ModelState.AddModelError("entity", Resources.ModelValidation_RequiredScoped);
                return Request.CreateBadRequestResult(Resources.BadRequestErrorMessage, ModelState);
            }

            if (!ModelState.IsValid)
                return Request.CreateBadRequestResult(Resources.BadRequestErrorMessage, ModelState);

            entity.Email = entity.Email.Trim().ToLowerInvariant();

            await _uow.BeginAsync(ct);
            var userManager = GetConfiguredApiUserManager(_uow);

            //    já existe algum utilizador para o email?
            ct.ThrowIfCancellationRequested();
            var user = await userManager.FindByNameAsync(entity.Email);
            if (user == null)
            {
                //    cria novo utilizador
                user = await AddNewUserAsync(
                    new ApiUser(), userManager, entity.Email, entity.Password,
                    entity.GivenName, entity.FamilyName, ct);
            }
            else if (user.EmailConfirmed)
            {
                //    se conta confirmada, as credenciais batem certo?
                //    (caso seja uma dupla submissão)
                ct.ThrowIfCancellationRequested();
                user = await userManager.FindAsync(entity.Email, entity.Password);
                if (user == null)
                {
                    ModelState.AddModelError(
                        "entity.Email", Resources.Conflict_Users_DuplicatedEmailScoped);
                    return Request.CreateConflictResponse(
                        Resources.Conflict_Authorizations_InvalidCredentials, ModelState);
                }
            }
            else
            {
                //    atualiza a password do utilizador para a nova
                //    (ocorre apenas caso o email não tenha sido ainda validado)
                user = await UpdateUserAsync(
                    user, userManager, entity.Email, entity.Password,
                    entity.GivenName, entity.FamilyName, ct);
            }

            await _uow.CommitAsync(ct);

            for (var i = 0; i < 3; i++)
            {
                try
                {
                    await SendConfirmationEmailAsync(user, userManager, ct);
                    break;
                }
                catch (Exception e)
                {
                    Logger.Warn(
                        "Failed to send a confirmation email for a new account " +
                        "[Attempt number: " + i + "]. Retrying...", e);
                }
                await Task.Delay(200, ct);
            }

            entity.Password = entity.PasswordConfirmation = string.Empty;
            entity.User = user;

            return Created(entity);
        }

        #endregion

        #region Private methods

        private UserManager<ApiUser> GetConfiguredApiUserManager(IALSUnitOfWork uow)
        {
            var userManager = uow.Security.GetUserManager<ApiUser>(false);
            ((UserValidator<ApiUser, string>)userManager.UserValidator)
                .AllowOnlyAlphanumericUserNames = false;
            userManager.UserTokenProvider = _userTokenProvider;
            userManager.EmailService = _emailService;

            return userManager;
        }

        private static async Task<TApiUser> AddNewUserAsync<TApiUser>(
            TApiUser user, UserManager<TApiUser, string> userManager, string email, string password,
            string givenName, string familyName, CancellationToken ct)
            where TApiUser : ApiUser
        {
            if (user == null) throw new ArgumentNullException("user");
            if (userManager == null) throw new ArgumentNullException("userManager");
            if (email == null) throw new ArgumentNullException("email");

            user.Id = user.UserName = user.Email = email;
            user.EmailConfirmed = false;
            user.CreatedOn = user.UpdatedOn = DateTimeOffset.Now;
            user.CreatedBy = user.UpdatedBy = "web.api";
            foreach (var roleId in Roles.DefaultRoles)
                user.Roles.Add(new IdentityUserRole { UserId = user.Id, RoleId = roleId });

            ct.ThrowIfCancellationRequested();
            var result = await userManager.CreateAsync(user, password);
            result.ThrowIfFailed("Failed to create the user");

            return user;
        }

        private static async Task<TApiUser> UpdateUserAsync<TApiUser>(
            TApiUser user, UserManager<TApiUser, string> userManager, string email, string password,
            string givenName, string familyName, CancellationToken ct)
            where TApiUser : ApiUser
        {
            if (user == null) throw new ArgumentNullException("user");
            if (userManager == null) throw new ArgumentNullException("userManager");
            if (email == null) throw new ArgumentNullException("email");

            ct.ThrowIfCancellationRequested();
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user.Id);

            ct.ThrowIfCancellationRequested();
            var result = await userManager.ResetPasswordAsync(user.Id, resetToken, password);
            result.ThrowIfFailed("Failed to reset the user password");

            user.Id = user.UserName = user.Email = email;
             user.EmailConfirmed = false;
            user.UpdatedOn = DateTimeOffset.Now;
            user.UpdatedBy = "web.api";
            foreach (var roleId in Roles.DefaultRoles
                .Where(roleId => user.Roles.All(e => e.RoleId != roleId)))
                user.Roles.Add(new IdentityUserRole { UserId = user.Id, RoleId = roleId });

            ct.ThrowIfCancellationRequested();
            result = await userManager.UpdateAsync(user);
            result.ThrowIfFailed("Failed to update the user");

            return user;
        }

        private async Task SendConfirmationEmailAsync<TApiUser>(
            TApiUser user, UserManager<TApiUser> userManager, CancellationToken ct)
            where TApiUser : ApiUser
        {
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user.Id);
            token = token.EncodeToBase64ASCII();

            var userId = user.Id.EncodeToBase64ASCII();

            var emailConfirmationUrl =
                Url.Link("Default",
                    new
                    {
                        Controller = "Account",
                        Action = "EmailConfirmation",
                        t = token,
                        e = userId
                    });

            await userManager.SendEmailConfirmationAsync(user.Id, emailConfirmationUrl, ct);
        }

        #endregion
    }

    public static class IdentityExtensions
    {
        public static void ThrowIfFailed(this IdentityResult result, string errorMessage)
        {
            if (result.Succeeded) return;

            var errors = result.Errors as string[] ?? result.Errors.ToArray();
            throw errors.Length > 1
                ? new AggregateException(errorMessage, result.Errors.Select(e => new Exception(e)))
                : new Exception(errors[0]);
        }
    }
}