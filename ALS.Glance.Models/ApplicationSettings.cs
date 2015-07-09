using System.Collections.Generic;
using ALS.Glance.Models.Core;
using ALS.Glance.Models.Security.Implementations;

namespace ALS.Glance.Models
{
    public class ApplicationSettings : ModelWithAllMeta<long>
    {
        public virtual string UserId { get; set; }

        public virtual IdentityUser User { get; set; }

        public virtual string ApplicationId { get; set; }

        public virtual ApplicationUser Application { get; set; }

        public virtual string Value { get; set; }
        public IDictionary<string, object> Values { get; set; }

    }
}
