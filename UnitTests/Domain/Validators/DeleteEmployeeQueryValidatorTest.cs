using Domain.Queries;
using Domain.Validators.QueryValidators;
using FluentValidation.Results;
using Xunit;


namespace UnitTests.Domain.Validators;

public class DeleteEmployeeQueryValidatorTest
{
    private readonly DeleteEmployeeQueryValidator validator = new();

    [Fact]
    public async Task ValidateAsync_Command_ReturnsValidResponse()
    {
        DeleteEmployeeQuery command = new DeleteEmployeeQuery
        {
            CorrelationId = Guid.NewGuid().ToString(),
            Id = Guid.NewGuid().ToString(),
        };
        ValidationResult result = await this.validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_Command_ReturnsInvalidResponse()
    {
        DeleteEmployeeQuery command = new DeleteEmployeeQuery
        {
            CorrelationId = Guid.NewGuid().ToString(),
            PageNo = 1,
            PageSize = 10,
        };
        ValidationResult result = await this.validator.ValidateAsync(command);
        Assert.False(result.IsValid);
    }
}