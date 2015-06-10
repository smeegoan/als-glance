using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;

namespace ALS.Glance.Api.Helpers.Routing
{
    public static class ODataRoutingExtensions
    {
        public static string GetEntitySetKeyActionName(this HttpControllerContext context, ODataPath odataPath, ILookup<string, HttpActionDescriptor> actionMap)
        {
            var entityRoutingConvention = new EntityRoutingConvention();
            var action = entityRoutingConvention
               .SelectAction(odataPath, context, actionMap);

            if (action == null)
            {
                return null;
            }
            var routeValues = context.RouteData.Values;

            object value;
            if (!routeValues.TryGetValue(ODataRouteConstants.Key,
              out value))
            {
                return action;
            }

            return !AddCompoundKeyValues(Convert.ToString(value), routeValues) ? null : action;
        }

        private static bool AddCompoundKeyValues(string compoundKey, IDictionary<string, object> routeValues)
        {
            var compoundKeyPairs = compoundKey.Split(',');

            if (!compoundKeyPairs.Any() || compoundKeyPairs.Count() < 2)
            {
                return false;
            }

            var keyValues = compoundKeyPairs
                .Select(kv =>
                    new
                    {
                        Value = kv,
                        SplitIndex = kv.IndexOf('=')
                    }
                ).Select(kv =>
                    new KeyValuePair<string, object>(
                        kv.Value.Substring(0, kv.SplitIndex),
                        kv.Value.Substring(kv.SplitIndex + 1, kv.Value.Length - kv.SplitIndex - 1)));

            foreach (var key in keyValues)
            {
                routeValues.Add(key.Key, key.Value);
            }
            return true;
        }

    }
}