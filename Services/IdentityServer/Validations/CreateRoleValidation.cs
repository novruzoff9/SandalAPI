using FluentValidation;
using IdentityServer.DTOs;

namespace IdentityServer.Validations;

public class CreateRoleValidation : AbstractValidator<CreateRoleDto>
{
    public CreateRoleValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Rol adı boş ola bilməz")
            .MaximumLength(100)
            .WithMessage("Rol adı 100 simvoldan çox ola bilməz");
    }
}