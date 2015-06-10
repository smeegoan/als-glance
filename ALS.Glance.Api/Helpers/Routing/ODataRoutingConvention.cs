using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;

namespace ALS.Glance.Api.Helpers.Routing
{
    public class ODataRoutingConvention : EntitySetRoutingConvention
    {
        public override string SelectAction(ODataPath odataPath, HttpControllerContext context,
            ILookup<string, HttpActionDescriptor> actionMap)
        {
            if (context.Request.Method != HttpMethod.Get
                && context.Request.Method != HttpMethod.Post
                && context.Request.Method != HttpMethod.Delete
                && context.Request.Method != HttpMethod.Put)
            {
                return null;
            }

            return odataPath.PathTemplate == "~/entityset/key" ? context.GetEntitySetKeyActionName(odataPath, actionMap) : null;
        }

    }
}