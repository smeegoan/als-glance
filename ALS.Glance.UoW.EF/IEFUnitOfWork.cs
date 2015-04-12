using ALS.Glance.UoW.Core;
using System;
using System.Data.Entity;

namespace ALS.Glance.UoW.EF
{
    public interface IEFUnitOfWork : IUnitOfWork, IDisposable
    {
        DbContext Context { get; }
    }
}
