using FluentValidation;

namespace TelephoneBookAssignment.Application.UserOperations.Commands.AddRoleToUser
{
    public class AddRoleToUserCommandValidator : AbstractValidator<AddRoleToUserCommand>
    {
        public AddRoleToUserCommandValidator()
        {
            RuleFor(x => x.Email).NotNull().NotEmpty();
            RuleFor(x => x.Role).NotNull().NotEmpty();
        }
    }
}