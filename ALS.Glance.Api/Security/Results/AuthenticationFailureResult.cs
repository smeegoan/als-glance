using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace ALS.Glance.Api.Security.Results
{
    public class AuthenticationFailureResult : IHttpActionResult
    {
        /// <summary>
        /// Creates a new AuthenticationFailureResult
        /// </summary>
        /// <param name="reasonPhrase"></param>
        /// <param name="request"></param>
        public AuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request)
        {
            _reasonPhrase = reasonPhrase;
            _request = request;
        }

        private readonly string _reasonPhrase;
        private readonly HttpRequestMessage _request;

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(
                new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    RequestMessage = _request,
                    ReasonPhrase = _reasonPhrase
                });
        }
    }
}