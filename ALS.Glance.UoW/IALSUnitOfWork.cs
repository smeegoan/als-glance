using ALS.Glance.UoW.Repository.Interface;
using ALS.Glance.UoW.Security.UnitOfWork.Interfaces;

namespace ALS.Glance.UoW
{
    public interface IALSUnitOfWork : ISecurityUnitOfWork
    {
        IApplicationSettingsRepository ApplicationSettings { get; }       
        IDateRepository Dates { get; }
        IMuscleRepository Muscles { get; }
        IFactRepository Facts { get; }

        IFactsRepository IndexedFacts { get; }
        IPatientRepository Patients { get; }
    }
}
