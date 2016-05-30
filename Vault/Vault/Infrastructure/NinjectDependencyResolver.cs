using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ninject;
using VaultDAL.Abstract;
using VaultDAL.Concrete;
using VaultDAL.Models;

namespace Vault.Infrastructure
{
    public class NinjectDependencyResolver:IDependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectDependencyResolver(IKernel kernel)
        {
            _kernel = kernel;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            _kernel.Bind<IConnectionProvider>().To<MongoConnectionProvider>();
            _kernel.Bind<IRepository<VaultDAL.Models.Vault>>().To<MongoRepository<VaultDAL.Models.Vault>>();
            _kernel.Bind<IRepository<VaultItem>>().To<MongoRepository<VaultItem>>();
            _kernel.Bind<IRepository<VaultUser>>().To<MongoRepository<VaultUser>>();
        }
    }
}