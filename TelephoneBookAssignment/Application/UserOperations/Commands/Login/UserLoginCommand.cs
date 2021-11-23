using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using TelephoneBookAssignment.Entities;
using TelephoneBookAssignment.Helpers;

namespace TelephoneBookAssignment.Application.UserOperations.Commands.Login
{
    public class UserLoginCommand
    {
        public UserLoginModel Model { get; set; } 
        private readonly IMongoCollection<User> _collection;

        public UserLoginCommand(IMongoCollection<User> collection)
        {
            _collection = collection;
        }

        public async Task<User> Handle()
        {
            var userToCheck = await _collection.Find(x => x.Email == Model.Email).FirstOrDefaultAsync();
            if (userToCheck is null)
            {
                throw new InvalidOperationException("User is not found!");
            }

            if (!HashingHelper.VerifyPasswordHash(Model.Password, userToCheck.PasswordHash, userToCheck.PasswordSalt))
            {
                throw new InvalidOperationException("Invalid Password!");
            }

            return userToCheck;
        }
    }

    public class UserLoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}