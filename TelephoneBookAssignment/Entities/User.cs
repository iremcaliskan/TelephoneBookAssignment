using System.Collections.Generic;
using TelephoneBookAssignment.Shared.Entities;

namespace TelephoneBookAssignment.Entities
{
    public class User : BaseMongoDbEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public List<UserClaim> Claims { get; set; }
    }

    public class UserClaim
    {
        public string Claim { get; set; }
    }
}