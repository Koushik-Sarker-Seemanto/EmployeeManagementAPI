using System.Linq.Expressions;
using Xunit;
using Moq;
using Domain.CommandHandlers;
using Domain.Commands;
using Dtos.Responses;
using Services.Abstraction;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Net;
using Dtos;
using Entities;
using Entities.Enums;

namespace UnitTests.Domain.CommandHandlers;

public class CreateEmployeeCommandHandlerTests
{
    private CreateEmployeeCommandHandler _handler;
    private readonly ILogger<CreateEmployeeCommandHandler> _logger;
    private readonly Mock<IValidationService> _validationServiceMock;
    private readonly IMapper _mapper;
    private readonly Mock<IEmployeeService> _employeeServiceMock;

    public CreateEmployeeCommandHandlerTests()
    {
        _logger = new Mock<ILogger<CreateEmployeeCommandHandler>>().Object;

        _mapper = this.SetupMockMapper();

        _employeeServiceMock = this.SetupEmployeeService();

        _validationServiceMock = this.SetupValidationService();

        _handler = new CreateEmployeeCommandHandler(_logger, _validationServiceMock.Object, _mapper, _employeeServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessResponse()
    {
        var command = new CreateEmployeeCommand
        {
            Email = "test@email.com",
            Department = Departments.Accounts,
            Name = "test",
            DoB = DateTime.Today,
        };
        var cancellationToken = new CancellationToken();

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ValidationResult.IsValid);
        Assert.NotNull(result.Result);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }
    
    [Fact]
    public async Task Handle_InValidCommand_ReturnsBadRequest()
    {
        // Arrange
        _validationServiceMock.Setup(x => x.ValidateAsync<CreateEmployeeCommand>(It.IsAny<CreateEmployeeCommand>()))
            .ReturnsAsync((CreateEmployeeCommand command) =>
            {
                ValidationResponse validationResponse = new ValidationResponse();
                validationResponse.AddError("Invalid payload", "Payload");
                return validationResponse;
            });
        
        _handler = new CreateEmployeeCommandHandler(_logger, _validationServiceMock.Object, _mapper, _employeeServiceMock.Object);
        
        var command = new CreateEmployeeCommand
        {
            Email = "test@email.com",
            Department = Departments.Accounts,
            Name = "test",
            DoB = DateTime.Today,
        };
        var cancellationToken = new CancellationToken();

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.ValidationResult.IsValid);
        Assert.Null(result.Result);
        Assert.Contains(result.ValidationResult.Errors, (x) => x.ErrorMessage.Equals("Invalid payload"));
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }
    
    [Fact]
    public async Task Handle_EmailAlreadyExists_ReturnsBadRequest()
    {
        // Arrange
        _employeeServiceMock.Setup(x => x.GetEmployeeAsync(It.IsAny<Expression<Func<Employee, bool>>>(), new CancellationToken()))
            .ReturnsAsync((Expression<Func<Employee, bool>> expression, CancellationToken cancellationToken) => new Employee());
        
        _handler = new CreateEmployeeCommandHandler(_logger, _validationServiceMock.Object, _mapper, _employeeServiceMock.Object);
        
        var command = new CreateEmployeeCommand
        {
            Email = "test@email.com",
            Department = Departments.Accounts,
            Name = "test",
            DoB = DateTime.Today,
        };
        var cancellationToken = new CancellationToken();

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.ValidationResult.IsValid);
        Assert.Null(result.Result);
        Assert.Contains(result.ValidationResult.Errors, (x) => x.ErrorMessage.Equals("Email already exists"));
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }
    
    [Fact]
    public async Task Handle_FailedToCreate_ReturnsInternalServerError()
    {
        // Arrange
        _employeeServiceMock.Setup(x => x.CreateEmployee(It.IsAny<string>(), It.IsAny<EmployeeDto>(), new CancellationToken()))
            .ReturnsAsync((string corrId, EmployeeDto employeeDto, CancellationToken cancellationToken) => null);
        
        _handler = new CreateEmployeeCommandHandler(_logger, _validationServiceMock.Object, _mapper, _employeeServiceMock.Object);
        
        var command = new CreateEmployeeCommand
        {
            Email = "test@email.com",
            Department = Departments.Accounts,
            Name = "test",
            DoB = DateTime.Today,
        };
        var cancellationToken = new CancellationToken();

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.ValidationResult.IsValid);
        Assert.Null(result.Result);
        Assert.Contains(result.ValidationResult.Errors, (x) => x.ErrorMessage.Equals("Failed to create employee"));
        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
    }
    
    [Fact]
    public async Task Handle_Exception_ReturnsInternalServerError()
    {
        // Arrange
        _employeeServiceMock.Setup(x => x.CreateEmployee(It.IsAny<string>(), It.IsAny<EmployeeDto>(), new CancellationToken()))
            .ReturnsAsync((string corrId, EmployeeDto employeeDto, CancellationToken cancellationToken) => throw new Exception("Force exception"));
        
        _handler = new CreateEmployeeCommandHandler(_logger, _validationServiceMock.Object, _mapper, _employeeServiceMock.Object);
        
        var command = new CreateEmployeeCommand
        {
            Email = "test@email.com",
            Department = Departments.Accounts,
            Name = "test",
            DoB = DateTime.Today,
        };
        var cancellationToken = new CancellationToken();

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.ValidationResult.IsValid);
        Assert.Null(result.Result);
        Assert.Contains(result.ValidationResult.Errors, (x) => x.ErrorMessage.Equals("Force exception"));
        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
    }

    private Mock<IValidationService> SetupValidationService()
    {
        Mock<IValidationService> obj = new Mock<IValidationService>();
        obj.Setup(x => x.ValidateAsync<CreateEmployeeCommand>(It.IsAny<CreateEmployeeCommand>()))
            .ReturnsAsync((CreateEmployeeCommand command) =>
            {
                ValidationResponse validationResponse = new ValidationResponse();
                return validationResponse;
            });

        return obj;
    }

    private IMapper SetupMockMapper()
    {
        Mock<IMapper> obj = new Mock<IMapper>();
        obj
            .Setup(x => x.Map<CreateEmployeeCommand, EmployeeDto>(It.IsAny<CreateEmployeeCommand>()))
            .Returns((CreateEmployeeCommand command) => new EmployeeDto()
            {
                Email = command.Email,
                Department = command.Department,
                Name = command.Name,
                DoB = command.DoB,
            });
        return obj.Object;
    }

    private Mock<IEmployeeService> SetupEmployeeService()
    {
        Mock<IEmployeeService> obj = new Mock<IEmployeeService>();
        obj
            .Setup(x => x.GetEmployeeAsync(It.IsAny<Expression<Func<Employee, bool>>>(), new CancellationToken()))
            .ReturnsAsync((Expression<Func<Employee, bool>> expression, CancellationToken cancellationToken) => null);
        
        obj
            .Setup(x => x.CreateEmployee(It.IsAny<string>(), It.IsAny<EmployeeDto>(), new CancellationToken()))
            .ReturnsAsync((string corrId, EmployeeDto employeeDto, CancellationToken cancellationToken) => employeeDto);

        return obj;
    }
}