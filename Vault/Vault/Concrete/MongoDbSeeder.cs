using AspNet.Identity.MongoDB;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;
using Vault.Abstract;
using Vault.Infrastructure;
using Vault.Models;

namespace Vault.Concrete    
{
    public class MongoDbSeeder:IDatabaseSeeder
    {
        private readonly IConnectionProvider _provider;
        private readonly AppIdentityDbContext _context;
        private AppUserManager UserManager => new AppUserManager(new UserStore<AppUserModel>(_context.Users));
        private AppRoleManager RoleManager => new AppRoleManager(new RoleStore<AppRoleModel>(_context.Roles));

        public MongoDbSeeder(IConnectionProvider provider, AppIdentityDbContext context)
        {
            _provider = provider;
            _context = context;
        }

        public void Seed()
        {
            var client = CreateMongoClient();
            Seed(client);
        }

        private void Seed(MongoClient client)
        {
            var database = client.GetDatabase(_provider.GetDatabase());
            RoleManager.Create(new AppRoleModel("Administrators"));
            var defaultUser = new AppUserModel()
            {
                UserName = "Admin",
                PasswordHash = UserManager.PasswordHasher.HashPassword("Aq12345")
            };
            UserManager.Create(defaultUser);
            var user = UserManager.FindByName(defaultUser.UserName);
            UserManager.AddToRole(user.Id, "Administrators");
        }

        private MongoClient CreateMongoClient()
        {
            return new MongoClient(_provider.GetServer());
        }
    }
}