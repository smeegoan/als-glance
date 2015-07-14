using System;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using ALS.Glance.Api.Properties;
using ALS.Glance.Api.Security.Extensions;
using ALS.Glance.Api.Security.Results;

namespace ALS.Glance.Api.Security.Filters
{
    /// <summary>
    /// Represents an api authentication filter
    /// </summary>
    public abstract class ApiAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        private readonly string _schemeId;

        /// <summary>
        /// This authentication realm
        /// </summary>
        public string Realm { get; set; }

        /// <summary>
        /// The challenge identification string
        /// </summary>
        public string SchemeId
        {
            get { return _schemeId; }
        }

        public virtual bool AllowMultiple
        {
            get { return false; }
        }

        /// <summary>
        /// Creates a new ApiAuthenticationAttribute identified by the given challenge id 
        /// </summary>
        /// <param name="schemeId">The challenge id</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected ApiAuthenticationAttribute(string schemeId)
        {
            if (schemeId == null)
                throw new ArgumentNullException("schemeId");

            _schemeId = schemeId;
        }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var request = context.Request;
            var authorization = request.Headers.Authorization;

            if (!IsAuthorizationHeaderValid(authorization))
                return;

            if (String.IsNullOrEmpty(authorization.Parameter))
            {
                context.ErrorResult = new AuthenticationFailureResult(Resources.EmptyCredentialsErrorMessage, request);
                return;
            }

            try
            {
                var principal = await AuthenticateAsync(authorization.Parameter, ct);
                if (principal == null)
                    context.ErrorResult = new AuthenticationFailureResult("Invalid username or password", request);
                else
                    context.Principal = principal;
            }
            catch (Exception e)
            {
                context.ErrorResult =
                    new AuthenticationFailureResult("Failed to authenticate the user: " + e.Message, request);
            }
        }

        /// <summary>
        /// Indicates if the authorization header is valid to be intercepted by this filter
        /// </summary>
        /// <param name="authorization"></param>
        /// <returns></returns>
        protected virtual bool IsAuthorizationHeaderValid(AuthenticationHeaderValue authorization)
        {
            return authorization != null &&
                   authorization.Scheme.Equals(_schemeId, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Authenticates the user using the authorization parameter
        /// </summary>
        /// <param name="authorizationParameter"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        protected abstract Task<IPrincipal> AuthenticateAsync(string authorizationParameter, CancellationToken ct);

        /// <summary>
        /// Challenges the context for this realm
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            context.ChallengeWith(
                _schemeId,
                String.IsNullOrWhiteSpace(Realm) ? null : "realm=\"" + Realm + "\"");

            return Task.FromResult(0);
        }
    }
}