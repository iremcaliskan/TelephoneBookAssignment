using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Threading.Tasks;
using TelephoneBookAssignment.Application.ContactOperations.Commands.CreateContact;
using TelephoneBookAssignment.Application.ContactOperations.Commands.DeleteContact;
using TelephoneBookAssignment.Application.ContactOperations.Commands.UpdateContact;
using TelephoneBookAssignment.Application.ContactOperations.Queries.GetContactDetails;
using TelephoneBookAssignment.Application.ContactOperations.Queries.GetContacts;
using TelephoneBookAssignment.Shared.DataAccess.MongoDb;
using TelephoneBookAssignment.Shared.Entities;

namespace TelephoneBookAssignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IMongoTelephoneBookDbContext _context;
        private readonly IMongoCollection<Contact> _dbCollection;
        private readonly IMapper _mapper;

        public ContactsController(IMongoTelephoneBookDbContext context, IMapper mapper)
        {
            _context = context;
            _dbCollection = _context.GetCollection<Contact>(nameof(Contact));
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetContacts()
        {
            GetContactsQuery query = new(_dbCollection, _mapper);

            var result = await query.Handle();

            return Ok(result);
        }

        [HttpGet("{objectId}")]
        public async Task<IActionResult> GetById(string objectId)
        {
            GetContactDetailsQuery query = new(_dbCollection, _mapper);
            query.ObjectId = objectId;

            GetContactDetailsQueryValidator validator = new();
            await validator.ValidateAndThrowAsync(query);

            var result = await query.Handle();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddContact([FromBody] CreateContactModel createContactModel)
        {
            CreateContactCommand command = new(_dbCollection, _mapper);
            command.Model = createContactModel;

            CreateContactCommandValidator validator = new();
            await validator.ValidateAndThrowAsync(command);

            await command.Handle();

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateContact(string objectId, [FromBody] UpdateContactModel updateContactModel)
        {
            UpdateContactCommand command = new(_dbCollection, _mapper);
            command.ObjectId = objectId;
            command.Model = updateContactModel;

            UpdateContactCommandValidator validator = new();
            await validator.ValidateAndThrowAsync(command);

            await command.Handle();

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteContact(string objectId)
        {
            DeleteContactCommand command = new(_dbCollection);
            command.ObjectId = objectId;

            DeleteContactCommandValidator validator = new();
            await validator.ValidateAndThrowAsync(command);

            await command.Handle();

            return Ok();
        }
    }
}