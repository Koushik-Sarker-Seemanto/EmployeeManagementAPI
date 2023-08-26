using Domain.Commands;
using Domain.Validators.CommandValidators;
using Entities.Enums;
using FluentValidation.Results;
using Xunit;


namespace UnitTests.Domain.Validators;

public class UpdateEmployeeCommandValidatorTest
{
    private readonly UpdateEmployeeCommandValidator validator = new();

    [Fact]
    public async Task ValidateAsync_Command_ReturnsValidResponse()
    {
        UpdateEmployeeCommand command = new UpdateEmployeeCommand
        {
            Id = Guid.NewGuid().ToString(),
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
        UpdateEmployeeCommand command = new UpdateEmployeeCommand
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