using FluentValidation;
using Registry.Models;

namespace Registry.Validator;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name must not be empty");
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number must not be empty");
    }
}
