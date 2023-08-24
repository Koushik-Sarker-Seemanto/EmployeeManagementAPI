using Domain.Queries;
using FluentValidation;

namespace Domain.Validators.QueryValidators;

public class GetEmployeeByIdQueryValidator : AbstractValidator<GetEmployeeByIdQuery>
{
    public GetEmployeeByIdQueryValidator()
    {
        RuleFor(e => e.Id).NotNull().NotEmpty();
    }
}