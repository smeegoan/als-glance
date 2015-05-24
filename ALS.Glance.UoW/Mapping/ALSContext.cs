using System.Data.Entity.Infrastructure.Annotations;
using ALS.Glance.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using ALS.Glance.UoW.EF;
using ALS.Glance.UoW.Security.Context.Implementation;

namespace ALS.Glance.UoW.Mapping
{
    public class ALSContext : SecurityDbContext
    {
        public ALSContext()
            : base("name=ALSContext")
        {

        }

        public ALSContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }


        public IDbSet<DDate> Date { get; set; }

        public IDbSet<Fact> Fact { get; set; }

        public IDbSet<DMuscle> Muscle { get; set; }

        public IDbSet<DPatient> Patient { get; set; }

        public IDbSet<DTime> Time { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DDate>(
                cfg =>
                {
                    cfg.ToTable("D_Date");
                    cfg.HasKey(x => x.Id);

                    cfg.Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                    cfg.Property(x => x.Date).IsRequired();
                    cfg.Property(x => x.Day).IsRequired();
                    cfg.Property(x => x.DayInMonth).IsRequired().HasMaxLength(50);
                    cfg.Property(x => x.Month).IsRequired();
                    cfg.Property(x => x.MonthName).IsRequired().HasMaxLength(50);
                    cfg.Property(x => x.Year).IsRequired();
                    cfg.Property(x => x.DayOfWeek).IsRequired().HasMaxLength(50);
                    cfg.Property(x => x.DayOfWeekName).IsRequired().HasMaxLength(50);
                    cfg.Property(x => x.Weekday).IsRequired().HasMaxLength(50);
                    cfg.Property(x => x.MonthInYear).IsRequired().HasMaxLength(50);
                    cfg.Property(x => x.Quarter).IsRequired();
                    cfg.Property(x => x.QuarterInYear).IsRequired().HasMaxLength(30);

                    cfg.Map();   
                });

            modelBuilder.Entity<DMuscle>(
                cfg =>
                {
                    cfg.ToTable("D_Muscle");
                    cfg.HasKey(x => x.Id);

                    cfg.Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                    cfg.Property(x => x.Name).IsRequired().IsUnicode(false).HasMaxLength(200);
                    cfg.Property(x => x.Abbreviation).IsRequired().HasMaxLength(30);

                    cfg.Map();   
                });

            modelBuilder.Entity<Fact>(
                cfg =>
                {
                    cfg.ToTable("Fact");
                    cfg.HasKey(x => x.Id);

                    cfg.Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                    cfg.Property(x => x.AUC).IsRequired();
                
                    cfg.HasRequired(a => a.Date).WithMany(b => b.Fact).HasForeignKey(c => c.DateId);
                    cfg.HasRequired(a => a.Muscle).WithMany(b => b.Fact).HasForeignKey(c => c.MuscleId);
                    cfg.HasRequired(a => a.Patient).WithMany(b => b.Fact).HasForeignKey(c => c.PatientId);
                    cfg.HasRequired(a => a.Time).WithMany(b => b.Fact).HasForeignKey(c => c.TimeId);
                    cfg.Map();   
                });

            modelBuilder.Entity<DPatient>(
                cfg =>
                {
                    cfg.ToTable("D_Patient");
                    cfg.HasKey(x => x.Id);

                    cfg.Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                    cfg.Property(x => x.Name).IsRequired().IsUnicode(false).HasMaxLength(500);
                    cfg.Property(x => x.BornOn).IsRequired();
                    cfg.Property(x => x.DiagnosedOn).IsRequired();
                    cfg.Property(x => x.PatientId).IsRequired().HasMaxLength(30);
                    cfg.Property(x => x.Sex).IsRequired().IsFixedLength().IsUnicode(false).HasMaxLength(1);

                    cfg.Map();   
                });

            modelBuilder.Entity<DTime>(
                cfg =>
                {
                    cfg.ToTable("D_Time");
                    cfg.HasKey(x => x.Id);

                    cfg.Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                    cfg.Property(x => x.Hour).IsRequired();
                    cfg.Property(x => x.TimeOfDay).IsRequired().IsUnicode(false).HasMaxLength(50);
                    cfg.Map();     
                });

            modelBuilder.Entity<ApplicationSettings>(
              cfg =>
              {
                  cfg.HasRequired(e => e.User).WithMany().HasForeignKey(e => e.UserId);
                  cfg.Property(e => e.UserId).HasColumnAnnotation(
                      IndexAnnotation.AnnotationName,
                      new IndexAnnotation(
                          new IndexAttribute("IX_dbo.ApplicationSettings_UserIdApplicationId", 1)
                          {
                              IsUnique = true
                          }));
                  cfg.HasRequired(e => e.Application).WithMany().HasForeignKey(e => e.ApplicationId);
                  cfg.Property(e => e.ApplicationId).HasColumnAnnotation(
                      IndexAnnotation.AnnotationName,
                      new IndexAnnotation(
                          new IndexAttribute("IX_dbo.ApplicationSettings_UserIdApplicationId", 2)
                          {
                              IsUnique = true
                          }));
                  cfg.Property(e => e.Value).HasMaxLength(4000);
                  cfg.Ignore(e => e.Values);
                  cfg.Map();
              });
            modelBuilder.Entity<Facts>(
                cfg =>
                {
                    cfg.ToTable( "Facts");
                
                    cfg.Property(x => x.Id).IsRequired();
                    cfg.Property(x => x.AUC).IsRequired().HasPrecision(20, 19);
                    cfg.Property(x => x.DateDate).IsRequired();
                    cfg.Property(x => x.DateDay).IsRequired();
                    cfg.Property(x => x.DateDayInMonth).IsRequired().HasMaxLength(50);
                    cfg.Property(x => x.DateMonth).IsRequired();
                    cfg.Property(x => x.DateMonthName).IsRequired().HasMaxLength(50);
                    cfg.Property(x => x.DateYear).IsRequired();
                    cfg.Property(x => x.DateDayOfWeek).IsRequired().HasMaxLength(50);
                    cfg.Property(x => x.DateDayOfWeekName).IsRequired().HasMaxLength(50);
                    cfg.Property(x => x.DateWeekday).IsRequired().HasMaxLength(50);
                    cfg.Property(x => x.DateMonthInYear).IsRequired().HasMaxLength(50);
                    cfg.Property(x => x.DateQuarter).IsRequired();
                    cfg.Property(x => x.DateQuarterInYear).IsRequired().HasMaxLength(30);
                    cfg.Property(x => x.MuscleAbbreviation).IsRequired().HasMaxLength(30);
                    cfg.Property(x => x.MuscleName).IsRequired().HasMaxLength(200);
                    cfg.Property(x => x.PatientId).IsRequired();
                    cfg.Property(x => x.PatientPatientId).IsRequired().HasMaxLength(30);
                    cfg.Property(x => x.PatientName).IsRequired().HasMaxLength(500);
                    cfg.Property(x => x.PatientSex).IsRequired().IsFixedLength().IsUnicode(false).HasMaxLength(1);
                    cfg.Property(x => x.PatientBornOn).IsRequired();
                    cfg.Property(x => x.PatientDiagnosedOn).IsRequired();
                    cfg.Property(x => x.TimeHour).IsRequired();
                    cfg.Property(x => x.TimeTimeOfDay).IsRequired().HasMaxLength(50);

                });
        }
    }

    /// <summary>
    /// Extension methods for code first configuration
    /// </summary>
    public static class EntityTypeConfigurationExtensions
    {
        /// <summary>
        /// Configures an entity using an action parameter
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="configurations"></param>
        /// <typeparam name="TEntityType"></typeparam>
        /// <returns></returns>
        public static EntityTypeConfiguration<TEntityType> Entity<TEntityType>(this DbModelBuilder modelBuilder, Action<EntityTypeConfiguration<TEntityType>> configurations)
            where TEntityType : class
        {
            var entityTypeConfiguration = modelBuilder.Entity<TEntityType>();
            configurations(entityTypeConfiguration);
            return entityTypeConfiguration;
        }
    }
}
