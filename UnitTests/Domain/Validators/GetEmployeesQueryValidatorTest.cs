using Domain.Queries;
using Domain.Validators.QueryValidators;
using Dtos.Enums;
using Entities.Enums;
using FluentValidation.Results;
using Xunit;


namespace UnitTests.Domain.Validators;

public class GetEmployeesQueryValidatorTest
{
    private readonly GetEmployeesQueryValidator validator = new();

    [Fact]
    public async Task ValidateAsync_Command_ReturnsValidResponse()
    {
        GetEmployeesQuery command = new GetEmployeesQuery
        {
            CorrelationId = Guid.NewGuid().ToString(),
            Name = "name",
            Department = Departments.Admin,
            SearchKey = "search",
            Email = "em",
            SortBy = "name",
            SortType = SortTypeEnum.DESC,
        };
        ValidationResult result = await this.validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }
}