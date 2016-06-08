using MongoDB.Driver;
using VaultDAL.Abstract;

namespace VaultService.Models
{
    public class IdentityHelper
    {
        public IMongoCollection<AppUser> Users; 
        public IdentityHelper()
        {
            Initiat();
        }

        private void Initiat()
        {
            IConnectionProvider provider = new IdentityConnectionProvider();
            var client = new MongoClient(provider.GetServer());
            var database = client.GetDatabase(provider.GetDatabase());
            this.Users = database.GetCollection<AppUser>("users");
        }
    }
}
