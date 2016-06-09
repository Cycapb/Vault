using Vault.Models;
using System.Threading.Tasks;
using System.Web.Mvc;
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vault.Controllers;

namespace Vault.Controllers.Tests
{
    /// <summary>This class contains parameterized unit tests for LogController</summary>
    [TestClass]
    [PexClass(typeof(LogController))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class LogControllerTest
    {

        /// <summary>Test stub for Index()</summary>
        [PexMethod]
        public ActionResult IndexTest([PexAssumeUnderTest]LogController target)
        {
            ActionResult result = target.Index();
            return result;
            // TODO: add assertions to method LogControllerTest.IndexTest(LogController)
        }

        /// <summary>Test stub for VaultLog(WebUser, String, String, Int32)</summary>
        [PexMethod]
        public Task<ActionResult> VaultLogTest(
            [PexAssumeUnderTest]LogController target,
            WebUser user,
            string id,
            string name,
            int page
        )
        {
            Task<ActionResult> result = target.VaultLog(user, id, name, page);
            return result;
            // TODO: add assertions to method LogControllerTest.VaultLogTest(LogController, WebUser, String, String, Int32)
        }
    }
}
