using FluentValidation;

namespace TelephoneBookAssignment.Application.ContactOperations.Commands.CreateContact
{
    public class CreateContactCommandValidator : AbstractValidator<CreateContactCommand>
    {
        public CreateContactCommandValidator()
        {
            RuleFor(x => x.Model.Name).MinimumLength(2);
            RuleFor(x => x.Model.LastName).MinimumLength(2);
            RuleFor(x => x.Model.Firm).MinimumLength(4);
            RuleFor(x => x.Model.Information.Location).MinimumLength(2);
            RuleFor(x => x.Model.Information.Mail).MinimumLength(9);
            RuleFor(x => x.Model.Information.PhoneNumber).MinimumLength(10);
        }
    }
}