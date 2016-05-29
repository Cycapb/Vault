using System;
using MongoDB.Driver;
using Vault.Models;

namespace Vault.Infrastructure
{
    public class AppIdentityDbContext:IDisposable
    {
        public IMongoCollection<AppUserModel> Users { get; set; } 
        public IMongoCollection<AppRoleModel> Roles { get; set; } 

        public static AppIdentityDbContext Create()
        {
            var client = new MongoClient("mongodb://192.168.1.144:27017");
            var database = client.GetDatabase("IdentityDb");
            var users = database.GetCollection<AppUserModel>("users");
            var roles = database.GetCollection<AppRoleModel>("roles");
            return new AppIdentityDbContext(users, roles);
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