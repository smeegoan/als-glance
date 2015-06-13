using System.Collections.Generic;
using ALS.Glance.Models.Core;
using ALS.Glance.Models.Security.Implementations;

namespace ALS.Glance.Models
{
    public class ApplicationSettings : ModelWithAllMeta<long>
    {
        public virtual string UserId { get; set; }

        public virtual ApiUser User { get; set; }

        public virtual string ApplicationId { get; set; }

        public virtual ApiApplicationUser Application { get; set; }

        public virtual string Value { get; set; }

    }
}
