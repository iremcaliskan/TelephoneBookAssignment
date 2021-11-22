using FluentValidation;

namespace TelephoneBookAssignment.Application.ContactOperations.Commands.DeleteContact
{
    public class DeleteContactCommandValidator : AbstractValidator<DeleteContactCommand>
    {
        public DeleteContactCommandValidator()
        {
            RuleFor(x => x.ObjectId).NotNull().NotEmpty();
        }
    }
}