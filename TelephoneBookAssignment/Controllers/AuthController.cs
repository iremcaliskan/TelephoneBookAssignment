using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Threading.Tasks;
using TelephoneBookAssignment.Application.UserOperations.Commands.AddRoleToUser;
using TelephoneBookAssignment.Application.UserOperations.Commands.CreateAccessToken;
using TelephoneBookAssignment.Application.UserOperations.Commands.Login;
using TelephoneBookAssignment.Application.UserOperations.Commands.Register;
using TelephoneBookAssignment.Application.UserOperations.Queries;
using TelephoneBookAssignment.Entities;
using TelephoneBookAssignment.Jwt;
using TelephoneBookAssignment.Shared.DataAccess.MongoDb;

namespace TelephoneBookAssignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMongoTelephoneBookDbContext _context;
        private readonly IMongoCollection<User> _userCollection;
        private readonly ITokenHelper _tokenHelper;
        private readonly IMapper _mapper;

        public AuthController(IMongoTelephoneBookDbContext context, ITokenHelper tokenHelper, IMapper mapper)
        {
            _context = context;
            _userCollection = _context.GetCollection<User>(nameof(Entities.User));
            _tokenHelper = tokenHelper;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginModel userLoginModel)
        {
            UserLoginCommand command = new(_userCollection);
            command.Model = userLoginModel;

            UserLoginCommandValidator validator = new();
            await validator.ValidateAndThrowAsync(command);

            var userToLogin = await command.Handle();

            if (userToLogin is null)
            {
                return BadRequest("Email or password is incorrect." );
            }

            CreateAccessTokenCommand accessTokenCommand = new(_userCollection, _tokenHelper);
            accessTokenCommand.ObjectId = userToLogin.ObjectId.ToString();

            var token = await accessTokenCommand.Handle();
            if (token is null)
            {
                return BadRequest();
            }

            return Ok(token);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterModel userRegisterModel)
        {
            UserExistsQuery query = new(_userCollection);
            query.Email = userRegisterModel.Email;

            var userExists = await query.Handle();

            if (userExists is true)
            {
                return BadRequest("User is existed in the system.");
            }

            UserRegisterCommand command = new(_userCollection, _mapper);
            command.Model = userRegisterModel;

            var registeredUser = await command.Handle();

            CreateAccessTokenCommand accessTokenCommand = new(_userCollection, _tokenHelper);
            accessTokenCommand.ObjectId = registeredUser.ObjectId.ToString();

            var token = await accessTokenCommand.Handle();

            if (token is null)
            {
                return BadRequest();
            }

            return Ok(token);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddRoleToUser(string email, string role)
        {
            UserExistsQuery query = new(_userCollection);
            query.Email = email;

            var userExists = await query.Handle();

            if (userExists is true)
            {
                AddRoleToUserCommand command = new(_userCollection);
                command.Email = email;
                command.Role = role;

                AddRoleToUserCommandValidator validator = new();
                await validator.ValidateAndThrowAsync(command);

                await command.Handle();

                return Ok("Role was added successfully");
            }
            return BadRequest();
        }
    }
}
/* Register
{
  "email": "test@test.com",
  "password": "test123",
  "firstName": "test",
  "lastName": "test",
  "username" : "test_test"
}
*/

/* Login
{
  "email": "test@test.com",
  "password": "test123"
}
*/