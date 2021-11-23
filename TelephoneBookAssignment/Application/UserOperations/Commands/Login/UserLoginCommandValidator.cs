using FluentValidation;

namespace TelephoneBookAssignment.Application.UserOperations.Commands.Login
{
    public class UserLoginCommandValidator : AbstractValidator<UserLoginCommand>
    {
        public UserLoginCommandValidator()
        {
            RuleFor(x => x.Model.Email).NotNull().NotEmpty();
            RuleFor(x => x.Model.Password).NotNull().NotEmpty();
        }
    }
}