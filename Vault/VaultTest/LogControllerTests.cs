using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VaultDAL.Models;
using  Moq;
using Vault.Abstract;
using Vault.Controllers;
using Vault.Models;

namespace VaultTest
{
    [TestClass]
    public class LogControllerTests
    {
        private readonly List<VaultAccessLog> _fakeLog = new List<VaultAccessLog>()
        {
            new VaultAccessLog(){ DateTime = DateTime.Today, Event = "E1", EventType = "Read", Id = "1", VaultId = "V1" },
            new VaultAccessLog(){ DateTime = DateTime.Today, Event = "E2", EventType = "Access", Id = "2", VaultId = "V2" },
            new VaultAccessLog(){ DateTime = DateTime.Today, Event = "E3", EventType = "Deny", Id = "4", VaultId = "V3" },
            new VaultAccessLog(){ DateTime = DateTime.Today, Event = "E4", EventType = "Full access", Id = "4", VaultId = "V2" },
        };


        [TestMethod]
        public async Task VaultLogInputUserNullNamePageReturnsView()
        {
            Mock<ILogManager<VaultAccessLog>> repoMock = new Mock<ILogManager<VaultAccessLog>>();
            repoMock.Setup(x => x.ShowLog(It.IsAny<string>())).ReturnsAsync(_fakeLog);
            var target = new LogController(repoMock.Object);

            var result = await target.VaultLog(new WebUser(), null);

            Assert.IsInstanceOfType(result,typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public async Task VaultLogInputUserIdNamePageReturnsView()
        {
            Mock<ILogManager<VaultAccessLog>> repoMock = new Mock<ILogManager<VaultAccessLog>>();
            repoMock.Setup(x => x.ShowLog(It.IsAny<string>())).ReturnsAsync(_fakeLog);
            var target = new LogController(repoMock.Object);

            var result = await target.VaultLog(new WebUser(), "E1");
            var model = ((ViewResult) result).ViewData.Model as VaultAccessLogModel;
            
            Assert.AreEqual(model.Events.Count(), 4);
            Assert.AreEqual(model.PagingInfo.CurrentPage,1);
            Assert.AreEqual(model.PagingInfo.TotalPages,1);
            Assert.AreEqual(model.PagingInfo.TotalItems,4);
        }
    }
}
