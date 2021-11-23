using AutoMapper;
using MongoDB.Driver;
using System.Threading.Tasks;
using FluentValidation;
using TelephoneBookAssignment.Application.UserOperations.Commands.CreateUser;
using TelephoneBookAssignment.Entities;
using TelephoneBookAssignment.Helpers;

namespace TelephoneBookAssignment.Application.UserOperations.Commands.Register
{
    public class UserRegisterCommand
    {
        public UserRegisterModel Model { get; set; }

        private readonly IMongoCollection<User> _collection;
        private readonly IMapper _mapper;

        public UserRegisterCommand(IMongoCollection<User> collection, IMapper mapper)
        {
            _collection = collection;
            _mapper = mapper;
        }

        public async Task<User> Handle()
        {
            // Hash the coming password and out its Hash and Salt
            HashingHelper.CreatePasswordHash(Model.Password, out var passwordHash, out var passwordSalt);

            var addedUser = new CreateUserModel
            {
                FirstName = Model.FirstName,
                LastName = Model.LastName,
                Username = Model.Username,
                Email = Model.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            CreateUserCommand command = new(_collection, _mapper) { Model = addedUser };

            CreateUserCommandValidator validator = new();
            await validator.ValidateAndThrowAsync(command);

            var user = await command.Handle();

            return user;
        }
    }

    public class UserRegisterModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}