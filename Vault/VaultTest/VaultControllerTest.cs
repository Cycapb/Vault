using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Vault.Abstract;
using Vault.Controllers;
using Vault.Models;
using VaultDAL.Abstract;
using VaultDAL.Models;

namespace VaultTest
{
    [TestClass]
    public class VaultControllerTest
    {
        [TestMethod]
        public void VaultListInputWebUserReturnsView()
        {
            Mock<IVaultManager> mockVaultManager = new Mock<IVaultManager>();
            var target = new VaultController(mockVaultManager.Object, null, null);

            var result = target.VaultList(new WebUser());

            mockVaultManager.Verify(x=>x.GetVaults(It.IsAny<string>()),Times.Once);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
        }

        [TestMethod]
        public void CreateInputWebUserReturnView()
        {
            Mock<IVaultManager> mockVaultManager = new Mock<IVaultManager>();
            var target = new VaultController(mockVaultManager.Object, null, null);

            var result = target.Create(new WebUser()) ;
            var model = (UserVault)((ViewResult)result).Model;

            Assert.IsNotNull(model);
            Assert.IsNotNull(model.AllowCreate);
            Assert.IsNotNull(model.AllowRead);
            Assert.IsInstanceOfType(result,typeof(ViewResult));
        }

        [TestMethod]
        public async Task CreateInputWebuserUservaultReturnView()
        {
            Mock<IVaultManager> mockVaultManager = new Mock<IVaultManager>();
            var target = new VaultController(mockVaultManager.Object, null, null);
            target.ModelState.AddModelError("","");

            var result = await target.Create(new WebUser() {Id = "1",UserName = "Admin"}, new UserVault());
            var model = ((ViewResult) result).Model as UserVault;

            Assert.IsNotNull(model);
            Assert.AreEqual(model.VaultAdmin.Id, "1");
            Assert.IsInstanceOfType(result,typeof(ViewResult));
        }

        [TestMethod]
        public async Task CreateInputWebuserUservaultReturnRedirect()
        {
            Mock<IVaultManager> mockVaultManager = new Mock<IVaultManager>();
            var target = new VaultController(mockVaultManager.Object, null, null);
            var user = new WebUser() {Id = "1", UserName = "Admin"};

            var result = await target.Create(user, new UserVault() {Name = "TestVault"});
            var model = ((RedirectToRouteResult) result);

            mockVaultManager.Verify(x => x.CreateAsync(It.IsAny<UserVault>()),Times.Once);
            Assert.IsInstanceOfType(result,typeof(RedirectToRouteResult));
            Assert.AreEqual(model.RouteValues["action"],"Index");
            Assert.AreEqual(target.TempData["success"], "Vault with name TestVault has been successfully created");
        }

        [TestMethod]
        public async Task CreateInputWebuserUservaultReturnError()
        {
            Mock<IVaultManager> mockVaultManager = new Mock<IVaultManager>();
            mockVaultManager.Setup(x => x.CreateAsync(It.IsAny<UserVault>())).ThrowsAsync(new SystemException());
            var target = new VaultController(mockVaultManager.Object, null, null);
            var user = new WebUser() { Id = "1", UserName = "Admin" };

            var result = await target.Create(user, new UserVault() { Name = "TestVault" });

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual(((ViewResult)result).ViewName,"Error");
        }

        [TestMethod]
        public async Task DeleteEqualUserIdsRedirectNoSuccess()
        {
            Mock<IVaultManager> mockVaultManager = new Mock<IVaultManager>();
            var vault = new UserVault() {Id = "1", VaultAdmin = new VaultUser() {Id = "1"}};
            mockVaultManager.Setup(x => x.GetVaultAdmin(vault.Id)).ReturnsAsync(vault.VaultAdmin);
            var target = new VaultController(mockVaultManager.Object, null ,null);

            var result = await target.Delete(new WebUser() {Id = "2"}, vault.Id);
            
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues["action"], "Index");
        }

        [TestMethod]
        public async Task DeleteNonequalUserIdsReturnRedirectSuccess()
        {
            Mock<IVaultManager> mockVaultManager = new Mock<IVaultManager>();
            var vault = new UserVault() { Id = "1", VaultAdmin = new VaultUser() { Id = "1" } };
            mockVaultManager.Setup(x => x.GetVaultAdmin(vault.Id)).ReturnsAsync(vault.VaultAdmin);
            var target = new VaultController(mockVaultManager.Object, null, null);

            var result = await target.Delete(new WebUser() {Id = "1"}, vault.Id);

            mockVaultManager.Verify(x => x.DeleteAsync(It.IsAny<string>()), Times.Once);
            Assert.IsNotNull(target.TempData["message"]);
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues["action"], "Index");
        }

        [TestMethod]
        public async Task DeleteGeneratesExceptionReturnView()
        {
            Mock<IVaultManager> mockVaultManager = new Mock<IVaultManager>();
            mockVaultManager.Setup(x => x.DeleteAsync(It.IsAny<string>())).Throws<SystemException>();
            var target = new VaultController(mockVaultManager.Object, null, null);

            var result = await target.Delete(new WebUser(), "1");

            Assert.IsInstanceOfType(result,typeof(ViewResult));
        }
    }
}
