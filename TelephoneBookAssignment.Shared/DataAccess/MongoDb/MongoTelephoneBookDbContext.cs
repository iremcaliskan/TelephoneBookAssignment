using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace TelephoneBookAssignment.Shared.DataAccess.MongoDb
{
    public class MongoTelephoneBookDbContext : IMongoTelephoneBookDbContext
    {
        private IMongoDatabase _mongoDatabase { get; set; }
        private MongoClient _mongoClient { get; set; }
        //public IClientSessionHandle Session { get; set; }

        public MongoTelephoneBookDbContext(IOptions<MongoSettings> configuration)
        {
            _mongoClient = new MongoClient(configuration.Value.ConnectionString);
            _mongoDatabase = _mongoClient.GetDatabase(configuration.Value.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _mongoDatabase.GetCollection<T>(name);
        }
    }
}