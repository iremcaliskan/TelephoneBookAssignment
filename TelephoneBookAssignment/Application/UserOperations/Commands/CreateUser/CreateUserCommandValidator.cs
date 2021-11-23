using FluentValidation;

namespace TelephoneBookAssignment.Application.UserOperations.Commands.CreateUser
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Model.FirstName).NotNull().NotEmpty();
            RuleFor(x => x.Model.LastName).NotNull().NotEmpty();
            RuleFor(x => x.Model.Username).NotNull().NotEmpty();
            RuleFor(x => x.Model.Email).NotNull().NotEmpty();
            RuleFor(x => x.Model.PasswordHash).NotNull();
            RuleFor(x => x.Model.PasswordSalt).NotNull();
        }
    }
}