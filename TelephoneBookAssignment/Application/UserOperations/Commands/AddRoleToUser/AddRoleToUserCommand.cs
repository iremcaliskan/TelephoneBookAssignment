using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using TelephoneBookAssignment.Entities;

namespace TelephoneBookAssignment.Application.UserOperations.Commands.AddRoleToUser
{
    public class AddRoleToUserCommand
    {
        public string Role { get; set; }
        public string Email { get; set; }

        private readonly IMongoCollection<User> _collection;

        public AddRoleToUserCommand(IMongoCollection<User> collection)
        {
            _collection = collection;
        }

        public async Task Handle()
        {
            var user = await _collection.Find(x => x.Email == Email).FirstOrDefaultAsync();
            if (user is null)
            {
                throw new InvalidOperationException("User is not found!");
            }

            if (user.Claims.Any(x => x.Claim.ToLower() == Role.ToLower()))
            {
                throw new InvalidOperationException("The user already has this role!");
            }

            user.Claims.Add(new UserClaim() { Claim = $"{Role.ToLower()}"});

            await _collection.FindOneAndReplaceAsync(x => x.Email == Email, user);
        }
    }
}