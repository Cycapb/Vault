using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Protocols;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Ninject;
using Vault.Abstract;
using Vault.Concrete;
using Vault.Models;
using VaultDAL.Abstract;
using VaultDAL.Concrete;
using VaultDAL.Models;


namespace Vault.Infrastructure
{
    public class NinjectDependencyResolver:IDependencyResolver
    {
        private readonly IKernel _kernel;
        private AppUserManager UserManager => HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();

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
            _kernel.Bind<IRepository<VaultAccessLog>>().To<MongoRepository<VaultAccessLog>>();
            _kernel.Bind<IVaultGetter>().To<UserVaultGetter>();
            _kernel.Bind<IVaultManager>().To<VaultManager>();
            _kernel.Bind<IUserGetter<VaultUser>>().To<FreeUsersGetter>();
            _kernel.Bind<IAccessManager>().To<AccessManager>();
            _kernel.Bind<IVaultItemManager>().To<VaultItemManager>();
            _kernel.Bind<IDbLogger>().To<DbLogger>();
            _kernel.Bind<ILogManager<VaultAccessLog>>().To<LogManager>();
            _kernel.Bind<ILogger>().To<FileLogger>();

            EmailSettings eSettings = new EmailSettings();
            _kernel.Bind<IMailReporter>().To<MailReporter>().WithConstructorArgument("emailSettings", eSettings);
        }
    }
}