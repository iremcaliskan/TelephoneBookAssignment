using FluentValidation;

namespace TelephoneBookAssignment.Application.UserOperations.Commands.Register
{
    public class UserRegisterCommandValidator : AbstractValidator<UserRegisterCommand>
    {
        public UserRegisterCommandValidator()
        {
            RuleFor(x => x.Model.FirstName).NotNull().NotEmpty();
            RuleFor(x => x.Model.LastName).NotNull().NotEmpty();
            RuleFor(x => x.Model.Username).NotNull().NotEmpty();
            RuleFor(x => x.Model.Email).NotNull().NotEmpty();
            RuleFor(x => x.Model.Password).NotNull().NotEmpty();
        }
    }
}