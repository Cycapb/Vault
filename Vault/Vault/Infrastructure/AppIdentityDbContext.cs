﻿using System;
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
            IExistingChecker checker = new MongoDbExistingChecker(provider);
            
            if (!checker.Exist(provider.GetDatabase()).Result)
            {
                 IDatabaseSeeder seeder = new MongoDbSeeder(provider);   
            }

            var client = new MongoClient(provider.GetServer());
            var database = client.GetDatabase(provider.GetDatabase());
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