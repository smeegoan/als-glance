using ALS.Glance.Models;
using ALS.Glance.UoW.Core;
using System.Threading;
using System.Threading.Tasks;

namespace ALS.Glance.UoW.Repository.Interface
{
    public interface IDateRepository : IRepository<DDate, long>
    {
   }
}