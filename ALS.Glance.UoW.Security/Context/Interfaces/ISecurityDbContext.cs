
using System;

namespace ALS.Glance.UoW.Security.Context.Interfaces
{
    public interface ISecurityDbContext : IDisposable
    {
        int Commit();
    }
}
