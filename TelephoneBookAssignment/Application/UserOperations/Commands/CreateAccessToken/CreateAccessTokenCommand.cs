using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;
using TelephoneBookAssignment.Application.UserOperations.Queries;
using TelephoneBookAssignment.Entities;
using TelephoneBookAssignment.Jwt;

namespace TelephoneBookAssignment.Application.UserOperations.Commands.CreateAccessToken
{
    public class CreateAccessTokenCommand
    {
        public string ObjectId { get; set; } // User objectId

        private readonly IMongoCollection<User> _userCollection;
        private readonly ITokenHelper _tokenHelper;

        public CreateAccessTokenCommand(IMongoCollection<User> userCollection, ITokenHelper tokenHelper)
        {
            _userCollection = userCollection;
            _tokenHelper = tokenHelper;
        }

        public async Task<AccessToken> Handle()
        {
            var user = await _userCollection.Find(x => x.ObjectId == new ObjectId(ObjectId)).FirstOrDefaultAsync();

            GetUserClaimsQuery query = new(_userCollection) { ObjectId = ObjectId };

            var claims = await query.Handle();

            var accessToken = _tokenHelper.CreateToken(user, claims);

            return accessToken;
        }
    }
}