using System.Data.Entity.ModelConfiguration;
using ALS.Glance.Models.Core;
using ALS.Glance.Models.Core.Interfaces;

namespace ALS.Glance.UoW.Security.Context.Implementation
{
    public static class FluentMapperExtensions
    {
        public static EntityTypeConfiguration<T> MapCreatedMeta<T>(this EntityTypeConfiguration<T> cfg)
            where T : class, IHaveCreatedMeta
        {
            cfg.Property(e => e.CreatedOn).IsRequired().HasColumnType("DATETIMEOFFSET").HasPrecision(7);
     
            return cfg;
        }

        public static EntityTypeConfiguration<T> MapUpdatedMeta<T>(this EntityTypeConfiguration<T> cfg)
            where T : class, IHaveUpdatedMeta
        {
            cfg.Property(e => e.UpdatedOn).IsRequired().HasColumnType("DATETIMEOFFSET").HasPrecision(7);
      
            return cfg;
        }

 


        public static EntityTypeConfiguration<T> MapCreatedAndUpdatedMeta<T>(this EntityTypeConfiguration<T> cfg)
            where T : class, IHaveCreatedMeta, IHaveUpdatedMeta
        {
            cfg.MapCreatedMeta();
            cfg.MapUpdatedMeta();

            return cfg;
        }
    }
}
