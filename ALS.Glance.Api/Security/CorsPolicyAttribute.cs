using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http.Cors;

namespace ALS.Glance.Api.Security
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class CorsPolicyAttribute : Attribute, ICorsPolicyProvider
    {
        private readonly CorsPolicy _policy;

        public CorsPolicyAttribute()
        {
            // Create a CORS policy.
            _policy = new CorsPolicy
            {
                AllowAnyMethod = true,
                AllowAnyHeader = true
            };

            // Add allowed origins.
            _policy.Origins.Add("http://localhost:53130");
            _policy.Origins.Add("http://als-glance-web.apphb.com");
        }

        public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request,CancellationToken ct)
        {
            return Task.FromResult(_policy);
        }
    }
}