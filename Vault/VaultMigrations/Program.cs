using System;
using VaultMigrations.Abstract;
using VaultMigrations.Concrete;
using VaultMigrations.Models;

namespace VaultMigrations
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Database initialization...");
            IConnectionProvider provider = new MongoConnectionProvider();
            AppIdentityDbContext context = AppIdentityDbContext.Create();
            InitDb(provider,context);
            Console.WriteLine("Migration finished");
            Console.ReadLine();
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

    }
}
