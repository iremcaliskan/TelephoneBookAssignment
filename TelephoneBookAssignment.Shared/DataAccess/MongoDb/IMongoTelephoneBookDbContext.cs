using MongoDB.Driver;

namespace TelephoneBookAssignment.Shared.DataAccess.MongoDb
{
    public interface IMongoTelephoneBookDbContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }
}