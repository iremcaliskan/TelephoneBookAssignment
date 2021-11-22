using AutoMapper;
using TelephoneBookAssignment.Application.ContactOperations.Commands.CreateContact;
using TelephoneBookAssignment.Application.ContactOperations.Commands.UpdateContact;
using TelephoneBookAssignment.Application.ContactOperations.Queries.GetContactDetails;
using TelephoneBookAssignment.Application.ContactOperations.Queries.GetContacts;
using TelephoneBookAssignment.Shared.Entities;

namespace TelephoneBookAssignment.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Contact:
            CreateMap<Contact, ContactViewModel>();
            CreateMap<Contact, ContactDetailViewModel>();
            CreateMap<CreateContactModel, Contact>();
            CreateMap<UpdateContactModel, Contact>();
        }
    }
}