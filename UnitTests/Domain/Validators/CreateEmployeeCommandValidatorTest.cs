using Domain.Commands;
using Domain.Validators.CommandValidators;
using Entities.Enums;
using FluentValidation.Results;
using Xunit;


namespace UnitTests.Domain.Validators;

public class CreateEmployeeCommandValidatorTest
{
    private readonly CreateEmployeeCommandValidator validator = new();

    [Fact]
    public async Task ValidateAsync_Command_ReturnsValidResponse()
    {
        CreateEmployeeCommand command = new CreateEmployeeCommand
        {
            CorrelationId = Guid.NewGuid().ToString(),
            Email = "test@email.com",
            Name = "test name",
            Department = Departments.Accounts,
            DoB = DateTime.Today,
        };
        ValidationResult result = await this.validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_Command_ReturnsInvalidResponse()
    {
        CreateEmployeeCommand command = new CreateEmployeeCommand
        {
            CorrelationId = Guid.NewGuid().ToString(),
            Name = "test name",
            Department = Departments.Accounts,
            DoB = DateTime.Today,
        };
        ValidationResult result = await this.validator.ValidateAsync(command);
        Assert.False(result.IsValid);
    }
}