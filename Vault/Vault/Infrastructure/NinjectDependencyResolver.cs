using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ninject;
using Vault.Abstract;
using Vault.Concrete;
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
            _kernel.Bind<IRepository<UserVault>>().To<MongoRepository<UserVault>>();
            _kernel.Bind<IRepository<VaultItem>>().To<MongoRepository<VaultItem>>();
            _kernel.Bind<IRepository<VaultUser>>().To<MongoRepository<VaultUser>>();
            _kernel.Bind<IVaultGetter>().To<UserVaultGetter>();
            _kernel.Bind<IVaultHelper>().To<VaultHelper>();
            _kernel.Bind<IUserGetter<VaultUser>>().To<FreeUsersGetter>();
        }
    }
}