using Domain.Queries;
using FluentValidation;

namespace Domain.Validators.QueryValidators;

public class GetEmployeesQueryValidator : AbstractValidator<GetEmployeesQuery>
{
    public GetEmployeesQueryValidator()
    {
    }
}