using ALS.Glance.UoW.Core;

namespace ALS.Glance.UoW.Security.UnitOfWork.Interfaces
{
    public interface ISecurityUnitOfWork : IUnitOfWork
    {
        ISecurityWorkArea Security { get; }
    }
}
