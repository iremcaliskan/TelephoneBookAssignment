using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using TelephoneBookAssignment.Shared.Entities;

namespace TelephoneBookAssignment.Application.ContactOperations.Commands.UpdateContact
{
    public class UpdateContactCommand
    {
        public string ObjectId { get; set; }
        public UpdateContactModel Model { get; set; }

        private readonly IMongoCollection<Contact> _collection;
        private readonly IMapper _mapper;

        public UpdateContactCommand(IMongoCollection<Contact> collection, IMapper mapper)
        {
            _collection = collection;
            _mapper = mapper;
        }

        public async Task Handle()
        {
            var contact = await _collection.Find(x => x.ObjectId == new ObjectId(ObjectId)).FirstOrDefaultAsync();
            if (contact is null)
            {
                throw new InvalidOperationException("No contact found to be updated!");
            }

            var vm = _mapper.Map<Contact>(Model);
            vm.ObjectId = contact.ObjectId;
            vm.Name = Model.Name != null ? Model.Name : contact.Name;
            vm.LastName = Model.LastName != null ? Model.LastName : contact.LastName;
            vm.Firm = Model.Firm != null ? Model.Firm : contact.Firm;
            vm.Information = Model.Information != null ? Model.Information : contact.Information;
            if (Model.Information != null)
            {
                vm.Information.Location = Model.Information.Location != null ? Model.Information.Location : contact.Information.Location;
                vm.Information.Mail = Model.Information.Mail != null ? Model.Information.Mail : contact.Information.Mail;
                vm.Information.PhoneNumber = Model.Information.PhoneNumber != null ? Model.Information.PhoneNumber : contact.Information.PhoneNumber;
            }
            await _collection.FindOneAndReplaceAsync(x => x.ObjectId == new ObjectId(ObjectId), vm);
        }
    }

    public class UpdateContactModel
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Firm { get; set; }
        public Contact.ContactInformation Information { get; set; }
    }
}