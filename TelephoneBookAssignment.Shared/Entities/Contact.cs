namespace TelephoneBookAssignment.Shared.Entities
{
    public class Contact : BaseMongoDbEntity
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Firm { get; set; }
        public ContactInformation Information { get; set; }

        public class ContactInformation
        {
            public string PhoneNumber { get; set; }
            public string Mail { get; set; }
            public string Location { get; set; }
        }
    }
}