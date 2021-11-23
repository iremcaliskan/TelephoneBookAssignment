using AutoMapper;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using TelephoneBookAssignment.Shared.Entities;

namespace TelephoneBookAssignment.Application.ContactOperations.Queries.GetContacts
{
    public class GetContactsQuery
    {
        private readonly IMongoCollection<Contact> _collection;
        private readonly IMapper _mapper;

        public GetContactsQuery(IMongoCollection<Contact> collection, IMapper mapper)
        {
            _collection = collection;
            _mapper = mapper;
        }


        public async Task<List<ContactViewModel>> Handle()
        {
            var contacts = await _collection.Find(x => true).ToListAsync();

            var vm = _mapper.Map<List<ContactViewModel>>(contacts);

            return vm;
        }
    }

    public class ContactViewModel
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Firm { get; set; }
        public Contact.ContactInformation Information { get; set; }
    }
}