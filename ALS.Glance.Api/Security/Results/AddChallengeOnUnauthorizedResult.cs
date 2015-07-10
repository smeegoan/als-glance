using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace ALS.Glance.Api.Security.Results
{
    public class AddChallengeOnUnauthorizedResult : IHttpActionResult
    {
        public AddChallengeOnUnauthorizedResult(AuthenticationHeaderValue challenge, IHttpActionResult innerResult)
        {
            if (challenge == null) throw new ArgumentNullException("challenge");
            if (innerResult == null) throw new ArgumentNullException("innerResult");

            _challenge = challenge;
            _innerResult = innerResult;
        }

        private readonly AuthenticationHeaderValue _challenge;

        private readonly IHttpActionResult _innerResult;

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = await _innerResult.ExecuteAsync(cancellationToken);
            if (
                response.StatusCode == HttpStatusCode.Unauthorized &&
                response.Headers.WwwAuthenticate.All(
                    h => h.Scheme.Equals(_challenge.Scheme, StringComparison.InvariantCultureIgnoreCase)))
                response.Headers.WwwAuthenticate.Add(_challenge);

            return response;
        }
    }
}