using FluentValidation;
using IdentityServer.DTOs;

namespace IdentityServer.Validations;

public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Zəhmət olmasa istifadəçinin adını daxil edin.")
            .Length(2, 50).WithMessage("İstifadəçinin adı 2 ilə 50 simvol arasında olmalıdır.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Zəhmət olmasa istifadəçinin soyadını daxil edin.")
            .Length(2, 50).WithMessage("İstifadəçinin soyadı 2 ilə 50 simvol arasında olmalıdır.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Zəhmət olmasa email ünvanını daxil edin.")
            .EmailAddress().WithMessage("Daxil edilən email ünvanı düzgün formatda deyil.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Zəhmət olmasa şifrəni daxil edin.")
            .MinimumLength(6).WithMessage("Şifrə ən azı 6 simvoldan ibarət olmalıdır.");
    }
}

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Zəhmət olmasa istifadəçinin adını daxil edin.")
            .Length(2, 50).WithMessage("İstifadəçinin adı 2 ilə 50 simvol arasında olmalıdır.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Zəhmət olmasa istifadəçinin soyadını daxil edin.")
            .Length(2, 50).WithMessage("İstifadəçinin soyadı 2 ilə 50 simvol arasında olmalıdır.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Zəhmət olmasa email ünvanını daxil edin.")
            .EmailAddress().WithMessage("Daxil edilən email ünvanı düzgün formatda deyil.");
    }
}
