using Domain.Commands;
using FluentValidation;

namespace Domain.Validators.CommandValidators;

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(e => e.Id).NotNull().NotEmpty();
        RuleFor(e => e.Name).NotNull().NotEmpty();
        RuleFor(e => e.Email).NotNull().NotEmpty();
    }
}