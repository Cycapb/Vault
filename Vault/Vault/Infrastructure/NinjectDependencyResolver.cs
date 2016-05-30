﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ninject;
using Vault.Abstract;
using Vault.Concrete;

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
            _kernel.Bind<IExistingChecker>().To<MongoDbExistingChecker>();
        }
    }
}