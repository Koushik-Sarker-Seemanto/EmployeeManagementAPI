using System.Net;
using Domain.QueryHandlers;
using Domain.Queries;
using Dtos;
using Dtos.Responses;
using Microsoft.Extensions.Logging;
using Moq;
using Services.Abstraction;
using Xunit;

namespace UnitTests.Domain.QueryHandlers;

public class DeleteEmployeeQueryHandlerTests
{
    private DeleteEmployeeQueryHandler _handler;
    private readonly ILogger<DeleteEmployeeQueryHandler> _logger;
    private readonly Mock<IEmployeeService> _employeeServiceMock;
    private readonly Mock<IValidationService> _validationServiceMock;

    public DeleteEmployeeQueryHandlerTests()
    {
        _logger = new Mock<ILogger<DeleteEmployeeQueryHandler>>().Object;

        _employeeServiceMock = this.SetupEmployeeService();

        _validationServiceMock = this.SetupValidationService();

        _handler = new DeleteEmployeeQueryHandler(_logger, _employeeServiceMock.Object, _validationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsEmployeeDto()
    {
        // Arrange
        var query = new DeleteEmployeeQuery
        {
            Id = "employee-id",
        };
        var cancellationToken = new CancellationToken();

        _employeeServiceMock.Setup(x => x.DeleteEmployee(It.IsAny<string>(), query.Id, cancellationToken))
            .ReturnsAsync((string corrId, string id, CancellationToken ct) =>
                (new EmployeeDto(), "Delete successful", HttpStatusCode.OK));

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
        _validationServiceMock.Setup(x => x.ValidateAsync<DeleteEmployeeQuery>(It.IsAny<DeleteEmployeeQuery>()))
            .ReturnsAsync((DeleteEmployeeQuery query) =>
            {
                ValidationResponse validationResponse = new ValidationResponse();
                validationResponse.AddError("Invalid payload", "Payload");
                return validationResponse;
            });

        _handler = new DeleteEmployeeQueryHandler(_logger, _employeeServiceMock.Object, _validationServiceMock.Object);

        var query = new DeleteEmployeeQuery
        {
            Id = "employee-id",
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
    public async Task Handle_FailedToDelete_ReturnsInternalServerError()
    {
        // Arrange
        _employeeServiceMock.Setup(x => x.DeleteEmployee(It.IsAny<string>(), It.IsAny<string>(), new CancellationToken()))
            .ReturnsAsync((string corrId, string id, CancellationToken cancellationToken) =>
                (null, "Delete failed", HttpStatusCode.InternalServerError));

        _handler = new DeleteEmployeeQueryHandler(_logger, _employeeServiceMock.Object, _validationServiceMock.Object);

        var query = new DeleteEmployeeQuery
        {
            Id = "employee-id",
        };
        var cancellationToken = new CancellationToken();

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.ValidationResult.IsValid);
        Assert.Null(result.Result);
        Assert.Contains(result.ValidationResult.Errors, (x) => x.ErrorMessage.Equals("Delete failed"));
        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
    }

    [Fact]
    public async Task Handle_Exception_ReturnsInternalServerError()
    {
        // Arrange
        _employeeServiceMock.Setup(x => x.DeleteEmployee(It.IsAny<string>(), It.IsAny<string>(), new CancellationToken()))
            .ReturnsAsync((string corrId, string id, CancellationToken cancellationToken) =>
                throw new Exception("Force exception"));

        _handler = new DeleteEmployeeQueryHandler(_logger, _employeeServiceMock.Object, _validationServiceMock.Object);

        var query = new DeleteEmployeeQuery
        {
            Id = "employee-id",
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
        obj.Setup(x => x.ValidateAsync<DeleteEmployeeQuery>(It.IsAny<DeleteEmployeeQuery>()))
            .ReturnsAsync((DeleteEmployeeQuery query) =>
            {
                ValidationResponse validationResponse = new ValidationResponse();
                return validationResponse;
            });

        return obj;
    }

    private Mock<IEmployeeService> SetupEmployeeService()
    {
        Mock<IEmployeeService> obj = new Mock<IEmployeeService>();
        obj
            .Setup(x => x.DeleteEmployee(It.IsAny<string>(), It.IsAny<string>(), new CancellationToken()))
            .ReturnsAsync((string corrId, string id, CancellationToken cancellationToken) =>
                (new EmployeeDto(), "Delete successful", HttpStatusCode.OK));

        return obj;
    }
}