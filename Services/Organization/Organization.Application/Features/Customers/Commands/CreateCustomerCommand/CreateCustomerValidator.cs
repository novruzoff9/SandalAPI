using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Application.Features.Customers.Commands.CreateCustomerCommand;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerValidator()
    {
        RuleFor(v => v.FirstName)
            .MaximumLength(50)
            .NotEmpty()
            .WithMessage("Name is required");

        RuleFor(v => v.LastName)
            .MaximumLength(60)
            .NotEmpty()
            .WithMessage("LastName is required");

        RuleFor(v => v.Email)
            .MaximumLength(200)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email is required");

        RuleFor(v => v.Phone)
            .NotEmpty().WithMessage("Phone is required")
            .Matches(@"^\+994(50|51|55|70|77|99|10|60|12)-\d{3}-\d{2}-\d{2}$")
            .WithMessage("Invalid phone number format. Expected format: +994xx-xxx-xx-xx")
            .WithErrorCode("InvalidPhoneNumberFormat");

    }
}
