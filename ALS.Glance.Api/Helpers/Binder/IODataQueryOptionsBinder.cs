using System;
using System.Collections.Generic;
using System.Web.OData.Query;

namespace ALS.Glance.Api.Helpers.Binder
{
    public interface IODataQueryOptionsBinder
    {
        ICollection<BinderNode> BindFilter(FilterQueryOption filterQuery, Action<Exception> exceptionHandler);
    }
}