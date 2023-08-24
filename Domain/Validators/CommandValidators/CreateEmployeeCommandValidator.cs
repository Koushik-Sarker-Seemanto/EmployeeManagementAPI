using Domain.Commands;
using FluentValidation;

namespace Domain.Validators.CommandValidators;

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(e => e.Name).NotNull().NotEmpty();
        RuleFor(e => e.Email).NotNull().NotEmpty();
    }
}