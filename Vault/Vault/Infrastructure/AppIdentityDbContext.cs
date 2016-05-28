using System;
using AspNet.Identity.MongoDB;
using MongoDB.Driver;
using Vault.Models;

namespace Vault.Infrastructure
{
    public class AppIdentityDbContext:IDisposable
    {
        public IMongoCollection<AppUserModel> Users { get; set; } 
        public IMongoCollection<IdentityRole> Roles { get; set; } 

        public static AppIdentityDbContext Create()
        {
            var client = new MongoClient("mongodb://192.168.1.144:27017");
            var database = client.GetDatabase("IdentityDb");
            var users = database.GetCollection<AppUserModel>("users");
            var roles = database.GetCollection<IdentityRole>("roles");
            return new AppIdentityDbContext(users, roles);
        }

        private AppIdentityDbContext(IMongoCollection<AppUserModel> users, IMongoCollection<IdentityRole> roles)
        {
            Users = users;
            Roles = roles;
        }

        public void Dispose()
        {
            
        }
    }
}