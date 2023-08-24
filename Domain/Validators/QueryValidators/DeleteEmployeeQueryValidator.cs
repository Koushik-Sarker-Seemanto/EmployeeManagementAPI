using Domain.Queries;
using FluentValidation;

namespace Domain.Validators.QueryValidators;

public class DeleteEmployeeQueryValidator : AbstractValidator<DeleteEmployeeQuery>
{
    public DeleteEmployeeQueryValidator()
    {
        RuleFor(e => e.Id).NotNull().NotEmpty();
    }
}