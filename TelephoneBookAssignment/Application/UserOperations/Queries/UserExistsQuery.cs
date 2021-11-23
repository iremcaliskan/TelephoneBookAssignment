using MongoDB.Driver;
using System.Threading.Tasks;
using TelephoneBookAssignment.Entities;

namespace TelephoneBookAssignment.Application.UserOperations.Queries
{
    public class UserExistsQuery
    {
        public string Email { get; set; }

        private readonly IMongoCollection<User> _userCollection;

        public UserExistsQuery(IMongoCollection<User> userCollection)
        {
            _userCollection = userCollection;
        }

        public async Task<bool> Handle()
        {
            var user = await _userCollection.Find(x => x.Email == Email).FirstOrDefaultAsync();
            if (user is not null)
            {
                return true;
            }

            return false;
        }
    }
}