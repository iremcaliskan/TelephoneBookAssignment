using TelephoneBookAssignment.Shared.Entities;

namespace TelephoneBookAssignment.Entities
{
    public class OperationClaim : BaseMongoDbEntity
    { // For showing claims
        public string Name { get; set; }
    }
}