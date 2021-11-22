using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using TelephoneBookAssignment.Shared.Entities;

namespace TelephoneBookAssignment.Application.ContactOperations.Queries.GetContactDetails
{
    public class GetContactDetailsQuery
    {
        public string ObjectId { get; set; }

        private readonly IMongoCollection<Contact> _collection;
        private readonly IMapper _mapper;

        public GetContactDetailsQuery(IMongoCollection<Contact> collection, IMapper mapper)
        {
            _collection = collection;
            _mapper = mapper;
        }

        public async Task<ContactDetailViewModel> Handle()
        {
           var contact = await _collection.Find(x => x.ObjectId == new ObjectId(ObjectId)).FirstOrDefaultAsync();
            if (contact is null)
            {
                throw new InvalidOperationException("Contact is not found!");
            }

            var vm = _mapper.Map<ContactDetailViewModel>(contact);

            return vm;
        }
    }

    public class ContactDetailViewModel
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Firm { get; set; }
        public Contact.ContactInformation Information { get; set; }
    }
}