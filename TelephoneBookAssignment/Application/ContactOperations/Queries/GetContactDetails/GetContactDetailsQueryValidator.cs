using FluentValidation;

namespace TelephoneBookAssignment.Application.ContactOperations.Queries.GetContactDetails
{
    public class GetContactDetailsQueryValidator : AbstractValidator<GetContactDetailsQuery>
    {
        public GetContactDetailsQueryValidator()
        {
            RuleFor(x => x.ObjectId).NotNull().NotEmpty();
        }
    }
}