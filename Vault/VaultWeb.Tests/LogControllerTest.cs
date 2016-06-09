// <copyright file="LogControllerTest.cs">Copyright ©  2016</copyright>
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vault.Abstract;
using Vault.Controllers;
using VaultDAL.Models;

namespace VaultWeb.Controllers.Tests
{
    /// <summary>This class contains parameterized unit tests for LogController</summary>
    [PexClass(typeof(LogController))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class LogControllerTest
    {
        /// <summary>Test stub for .ctor(ILogManager`1&lt;VaultAccessLog&gt;)</summary>
        [PexMethod]
        public LogController ConstructorTest(ILogManager<VaultAccessLog> logManager)
        {
            LogController target = new LogController(logManager);
            return target;
            // TODO: add assertions to method LogControllerTest.ConstructorTest(ILogManager`1<VaultAccessLog>)
        }
    }
}
