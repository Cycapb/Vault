using System;
using MongoDB.Driver;
using Vault.Abstract;
using Vault.Concrete;
using Vault.Models;

namespace Vault.Infrastructure
{
    public class AppIdentityDbContext:IDisposable
    {
        public IMongoCollection<AppUserModel> Users { get; set; } 
        public IMongoCollection<AppRoleModel> Roles { get; set; }

        public static AppIdentityDbContext Create()
        {
            IConnectionProvider provider = new MongoConnectionProvider();
            var client = new MongoClient(provider.GetServer());
            var database = client.GetDatabase(provider.GetDatabase());
            var users = database.GetCollection<AppUserModel>("users");
            var roles = database.GetCollection<AppRoleModel>("roles");
            AppIdentityDbContext context = new AppIdentityDbContext(users, roles);
            InitDb(provider,context);
            return context;
        }

        private static void InitDb(IConnectionProvider provider, AppIdentityDbContext context)
        {
            IExistingChecker checker = new MongoDbExistingChecker(provider);
            if (!checker.Exist(provider.GetDatabase()).Result)
            {
                IDatabaseSeeder seeder = new MongoDbSeeder(provider, context);
                seeder.Seed();
            }
        }

        private AppIdentityDbContext(IMongoCollection<AppUserModel> users, IMongoCollection<AppRoleModel> roles)
        {
            Users = users;
            Roles = roles;
        }

        public void Dispose()
        {
            
        }
    }
}