using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using TelephoneBookAssignment.Shared.Entities;

namespace TelephoneBookAssignment.Application.ContactOperations.Commands.DeleteContact
{
    public class DeleteContactCommand
    {
        public string ObjectId { get; set; }

        private readonly IMongoCollection<Contact> _collection;

        public DeleteContactCommand(IMongoCollection<Contact> collection)
        {
            _collection = collection;
        }

        public async Task Handle()
        {
            var contact = await _collection.Find(x => x.ObjectId == new ObjectId(ObjectId)).FirstOrDefaultAsync();
            if (contact is null)
            {
                throw new InvalidOperationException("No contact found to be deleted!");
            }

            await _collection.FindOneAndDeleteAsync(x => x.ObjectId == new ObjectId(ObjectId));
        }
    }
}