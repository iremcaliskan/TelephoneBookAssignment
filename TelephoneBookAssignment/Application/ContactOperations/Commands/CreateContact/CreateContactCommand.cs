using AutoMapper;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using TelephoneBookAssignment.Shared.Entities;

namespace TelephoneBookAssignment.Application.ContactOperations.Commands.CreateContact
{
    public class CreateContactCommand
    {
        public CreateContactModel Model { get; set; }

        private readonly IMongoCollection<Contact> _collection;
        private readonly IMapper _mapper;
        public CreateContactCommand(IMongoCollection<Contact> collection, IMapper mapper)
        {
            _collection = collection;
            _mapper = mapper;
        }

        public async Task Handle()
        {
            Contact contact;

            if (!string.IsNullOrEmpty(Model.Information.PhoneNumber))
            { 
                contact = await _collection.Find(x => x.Information.PhoneNumber == Model.Information.PhoneNumber).FirstOrDefaultAsync();
                if (contact is not null)
                {
                    throw new InvalidOperationException("The contact is already in the system.");
                }
            }
            
            contact = _mapper.Map<Contact>(Model);

            await _collection.InsertOneAsync(contact);
        }
    }

    public class CreateContactModel
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Firm { get; set; }
        public Contact.ContactInformation Information { get; set; }
    }
}