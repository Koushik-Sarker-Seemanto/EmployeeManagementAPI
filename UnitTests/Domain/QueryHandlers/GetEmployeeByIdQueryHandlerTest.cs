using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using Domain.QueryHandlers;
using Domain.Queries;
using Dtos;
using Dtos.Responses;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Services.Abstraction;
using Xunit;

namespace UnitTests.Domain.QueryHandlers;

public class GetEmployeeByIdQueryHandlerTests
{
    private GetEmployeeByIdQueryHandler _handler;
    private readonly ILogger<GetEmployeeByIdQueryHandler> _logger;
    private readonly Mock<IEmployeeService> _employeeServiceMock;
    private readonly IMapper _mapper;
    private readonly Mock<IValidationService> _validationServiceMock;

    public GetEmployeeByIdQueryHandlerTests()
    {
        _logger = new Mock<ILogger<GetEmployeeByIdQueryHandler>>().Object;

        _mapper = this.SetupMockMapper();

        _employeeServiceMock = this.SetupEmployeeService();

        _validationServiceMock = this.SetupValidationService();

        _handler = new GetEmployeeByIdQueryHandler(_logger, _employeeServiceMock.Object, _mapper, _validationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsEmployeeDto()
    {
        // Arrange
        var query = new GetEmployeeByIdQuery
        {
            Id = "employee-id",
            // ... other properties
        };
        var cancellationToken = new CancellationToken();

        _employeeServiceMock.Setup(x => x.GetEmployeeAsync(It.IsAny<Expression<Func<Employee, bool>>>(), cancellationToken))
            .ReturnsAsync((Expression<Func<Employee, bool>> expression, CancellationToken ct) =>
                new Employee { Id = "employee-id", /* ... other properties */ });

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ValidationResult.IsValid);
        Assert.NotNull(result.Result);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(1, result.Count);
    }

    [Fact]
    public async Task Handle_InvalidQuery_ReturnsBadRequest()
    {
        // Arrange
        _validationServiceMock.Setup(x => x.ValidateAsync<GetEmployeeByIdQuery>(It.IsAny<GetEmployeeByIdQuery>()))
            .ReturnsAsync((GetEmployeeByIdQuery query) =>
            {
                ValidationResponse validationResponse = new ValidationResponse();
                validationResponse.AddError("Invalid payload", "Payload");
                return validationResponse;
            });

        _handler = new GetEmployeeByIdQueryHandler(_logger, _employeeServiceMock.Object, _mapper, _validationServiceMock.Object);

        var query = new GetEmployeeByIdQuery
        {
            Id = "employee-id",
            // ... other properties
        };
        var cancellationToken = new CancellationToken();

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.ValidationResult.IsValid);
        Assert.Null(result.Result);
        Assert.Contains(result.ValidationResult.Errors, (x) => x.ErrorMessage.Equals("Invalid payload"));
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Handle_NoEmployeeFound_ReturnsNotFound()
    {
        // Arrange
        var query = new GetEmployeeByIdQuery
        {
            Id = "non-existent-employee-id",
            // ... other properties
        };
        var cancellationToken = new CancellationToken();

        _employeeServiceMock.Setup(x => x.GetEmployeeAsync(It.IsAny<Expression<Func<Employee, bool>>>(), cancellationToken))
            .ReturnsAsync((Expression<Func<Employee, bool>> expression, CancellationToken ct) => null);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.ValidationResult.IsValid);
        Assert.Null(result.Result);
        Assert.Contains(result.ValidationResult.Errors, (x) => x.ErrorMessage.Equals("No employee found with this id"));
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task Handle_Exception_ReturnsInternalServerError()
    {
        // Arrange
        _employeeServiceMock.Setup(x => x.GetEmployeeAsync(It.IsAny<Expression<Func<Employee, bool>>>(), new CancellationToken()))
            .ReturnsAsync((Expression<Func<Employee, bool>> expression, CancellationToken cancellationToken) =>
                throw new Exception("Force exception"));

        _handler = new GetEmployeeByIdQueryHandler(_logger, _employeeServiceMock.Object, _mapper, _validationServiceMock.Object);

        var query = new GetEmployeeByIdQuery
        {
            Id = "employee-id",
            // ... other properties
        };
        var cancellationToken = new CancellationToken();

        // Act
        var result = await _handler.Handle(query, cancellationToken);

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
        obj.Setup(x => x.ValidateAsync<GetEmployeeByIdQuery>(It.IsAny<GetEmployeeByIdQuery>()))
            .ReturnsAsync((GetEmployeeByIdQuery query) =>
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
            .Setup(x => x.Map<Employee, EmployeeDto>(It.IsAny<Employee>()))
            .Returns((Employee employee) => new EmployeeDto()
            {
                // ... map properties
            });
        return obj.Object;
    }

    private Mock<IEmployeeService> SetupEmployeeService()
    {
        Mock<IEmployeeService> obj = new Mock<IEmployeeService>();
        obj
            .Setup(x => x.GetEmployeeAsync(It.IsAny<Expression<Func<Employee, bool>>>(), new CancellationToken()))
            .ReturnsAsync((Expression<Func<Employee, bool>> expression, CancellationToken cancellationToken) => null);
            
        // Setup for returning an existing employee as needed

        return obj;
    }
}