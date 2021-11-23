using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TelephoneBookAssignment.Entities;

namespace TelephoneBookAssignment.Application.UserOperations.Queries
{
    public class GetUserClaimsQuery
    {
        public string ObjectId { get; set; } // User objectId

        private readonly IMongoCollection<User> _userCollection;

        public GetUserClaimsQuery(IMongoCollection<User> userCollection)
        {
            _userCollection = userCollection;
        }

        public async Task<List<UserClaim>> Handle()
        {
            var user = await _userCollection.Find(x => x.ObjectId == new ObjectId(ObjectId)).FirstOrDefaultAsync();
            if (user is null)
            {
                throw new InvalidOperationException("User is not found!");
            }

            var userClaims = user.Claims;

            return userClaims;
        }
    }
}