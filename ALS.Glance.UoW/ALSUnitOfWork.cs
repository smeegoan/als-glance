using ALS.Glance.UoW.EF;
using ALS.Glance.UoW.Mapping;
using ALS.Glance.UoW.Repository.Implementation;
using ALS.Glance.UoW.Repository.Interface;
using System;
using ALS.Glance.UoW.Security.UnitOfWork.Implementation;

namespace ALS.Glance.UoW
{
    public class ALSUnitOfWork : SecurityUnitOfWork, IALSUnitOfWork
    {
         private readonly Lazy<IDateRepository> _lazyDateRepository;
        private readonly Lazy<IMuscleRepository> _lazyMuscleRepository;
        private readonly Lazy<IFactRepository> _lazyFactRepository;
        private readonly Lazy<IFactsRepository> _lazyFactsRepository;
        private readonly Lazy<IPatientRepository> _lazyPatientRepository;
        private readonly Lazy<IApplicationSettingsRepository> _lazyApplicationSettingsRepository;

        public ALSUnitOfWork(ALSContext context)
            : base(context)
        {
            _lazyDateRepository = new Lazy<IDateRepository>(() => new DateRepository(context));
            _lazyMuscleRepository = new Lazy<IMuscleRepository>(() => new MuscleRepository(context));
            _lazyFactRepository = new Lazy<IFactRepository>(() => new FactRepository(context));
            _lazyFactsRepository = new Lazy<IFactsRepository>(() => new FactsRepository(context));
            _lazyPatientRepository = new Lazy<IPatientRepository>(() => new PatientRepository(context));
            _lazyApplicationSettingsRepository = new Lazy<IApplicationSettingsRepository>(() => new ApplicationSettingsRepository(context));
        }

        public IDateRepository Dates { get { return _lazyDateRepository.Value; } }
        public IMuscleRepository Muscles { get { return _lazyMuscleRepository.Value; } }
        public IFactsRepository IndexedFacts { get { return _lazyFactsRepository.Value; } }
        public IFactRepository Facts { get { return _lazyFactRepository.Value; } }
        public IPatientRepository Patients { get { return _lazyPatientRepository.Value; } }
        public IApplicationSettingsRepository ApplicationSettings { get { return _lazyApplicationSettingsRepository.Value; } }

    }
}