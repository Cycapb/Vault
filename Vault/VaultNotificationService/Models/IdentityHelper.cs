using MongoDB.Driver;
using VaultDAL.Abstract;

namespace VaultService.Models
{
    public class IdentityHelper
    {
        public IMongoCollection<AppUser> Users; 
        public IdentityHelper()
        {
            Initiate();
        }

        private void Initiate()
        {
            IConnectionProvider provider = new IdentityConnectionProvider();
            var client = new MongoClient(provider.GetServer());
            var database = client.GetDatabase(provider.GetDatabase());
            this.Users = database.GetCollection<AppUser>("users");
        }
    }
}
