using System;

namespace ALS.Glance.UoW.Core
{
    public interface IUnitOfWorkFactory
    {
        TUoW Get<TUoW>() where TUoW : IUnitOfWork;
        IUnitOfWork Get(Type uowType);
        void Release(IUnitOfWork unitOfWork);
    }
}