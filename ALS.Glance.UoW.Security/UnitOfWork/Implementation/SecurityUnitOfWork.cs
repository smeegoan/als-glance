using System;
using ALS.Glance.UoW.EF;
using ALS.Glance.UoW.Security.UnitOfWork.Interfaces;
using ALS.Glance.UoW.Security.Context.Implementation;

namespace ALS.Glance.UoW.Security.UnitOfWork.Implementation
{
    public class SecurityUnitOfWork : EFUnitOfWork, ISecurityUnitOfWork
    {
        private readonly Lazy<ISecurityWorkArea> _securityWorkArea;

        public ISecurityWorkArea Security
        {
            get { return _securityWorkArea.Value; }
        }

        public SecurityUnitOfWork(SecurityDbContext context)
            : base(context)
        {
            if (context == null) throw new ArgumentNullException("context");
            _securityWorkArea = new Lazy<ISecurityWorkArea>(() => new SecurityWorkArea(context));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disposable = _securityWorkArea.Value as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
            base.Dispose(disposing);
        }
    }

}
