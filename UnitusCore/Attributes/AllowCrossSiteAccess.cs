using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Cors;
using System.Web.Http.Cors;
using System.Web.Http.Filters;

namespace UnitusCore.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class UnitusCorsEnabledAttribute : Attribute, ICorsPolicyProvider
    {

        private CorsPolicy _policy;

        public UnitusCorsEnabledAttribute()
        {
            _policy=new CorsPolicy()
            {
                AllowAnyHeader = true,
                AllowAnyMethod = true,
                SupportsCredentials = true
            };
            _policy.Origins.Add("https://localhost");
            _policy.Origins.Add("https://localhost:44301");
            _policy.Origins.Add("https://unitus-ac.com");
            _policy.Origins.Add("http://unitus-ac.com");
        }

        public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_policy);
        }
    }

    [Flags]
    public enum AccessFrom
    {
        LocalHost=0x01,
        Unitus=0x02,
        All=0x04
    }
}