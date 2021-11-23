using AutoMapper;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TelephoneBookAssignment.Entities;

namespace TelephoneBookAssignment.Application.UserOperations.Commands.CreateUser
{
    public class CreateUserCommand
    {
        public CreateUserModel Model { get; set; }

        private readonly IMongoCollection<User> _collection;
        private readonly IMapper _mapper;

        public CreateUserCommand(IMongoCollection<User> collection, IMapper mapper)
        {
            _collection = collection;
            _mapper = mapper;
        }

        public async Task<User> Handle()
        {
            User user;
            if (!string.IsNullOrEmpty(Model.Email))
            {
                user = await _collection.Find(x => x.Email == Model.Email).FirstOrDefaultAsync();
                if (user is not null)
                {
                    throw new InvalidOperationException("The user is already in the system.");
                }
            }

            user = _mapper.Map<User>(Model);

            // For all system users:
            user.Claims = new List<UserClaim>() {new UserClaim() {Claim = "default"}};

            await _collection.InsertOneAsync(user);

            return user;
        }
    }

    public class CreateUserModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}