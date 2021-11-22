using FluentValidation;

namespace TelephoneBookAssignment.Application.ContactOperations.Commands.UpdateContact
{
    public class UpdateContactCommandValidator : AbstractValidator<UpdateContactCommand>
    {
        public UpdateContactCommandValidator()
        {
            RuleFor(x => x.ObjectId).NotNull().NotEmpty();
            RuleFor(x => x.Model.Name).MinimumLength(2);
            RuleFor(x => x.Model.LastName).MinimumLength(2);
            RuleFor(x => x.Model.Firm).MinimumLength(4);
        }
    }
}