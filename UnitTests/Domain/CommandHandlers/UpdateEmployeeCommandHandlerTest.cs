using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using Domain.CommandHandlers;
using Domain.Commands;
using Dtos;
using Dtos.Responses;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Services.Abstraction;
using Xunit;

namespace UnitTests.Domain.CommandHandlers
{
    public class UpdateEmployeeCommandHandlerTests
    {
        private UpdateEmployeeCommandHandler _handler;
        private readonly ILogger<UpdateEmployeeCommandHandler> _logger;
        private readonly Mock<IValidationService> _validationServiceMock;
        private readonly IMapper _mapper;
        private readonly Mock<IEmployeeService> _employeeServiceMock;

        public UpdateEmployeeCommandHandlerTests()
        {
            _logger = new Mock<ILogger<UpdateEmployeeCommandHandler>>().Object;

            _mapper = this.SetupMockMapper();

            _employeeServiceMock = this.SetupEmployeeService();

            _validationServiceMock = this.SetupValidationService();

            _handler = new UpdateEmployeeCommandHandler(_logger, _validationServiceMock.Object, _mapper, _employeeServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccessResponse()
        {
            // Arrange
            var command = new UpdateEmployeeCommand
            {
                Id = "employee-id",
                Email = "test@email.com",
                // ... other properties
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
        public async Task Handle_InvalidCommand_ReturnsBadRequest()
        {
            // Arrange
            _validationServiceMock.Setup(x => x.ValidateAsync<UpdateEmployeeCommand>(It.IsAny<UpdateEmployeeCommand>()))
                .ReturnsAsync((UpdateEmployeeCommand command) =>
                {
                    ValidationResponse validationResponse = new ValidationResponse();
                    validationResponse.AddError("Invalid payload", "Payload");
                    return validationResponse;
                });

            _handler = new UpdateEmployeeCommandHandler(_logger, _validationServiceMock.Object, _mapper, _employeeServiceMock.Object);

            var command = new UpdateEmployeeCommand
            {
                Id = "employee-id",
                Email = "test@email.com",
                // ... other properties
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
        public async Task Handle_EmailAlreadyExists_ReturnsInternalServerError()
        {
            // Arrange
            _employeeServiceMock.Setup(x => x.GetEmployeeAsync(It.IsAny<Expression<Func<Employee, bool>>>(), new CancellationToken()))
                .ReturnsAsync((Expression<Func<Employee, bool>> expression, CancellationToken cancellationToken) => new Employee());

            _handler = new UpdateEmployeeCommandHandler(_logger, _validationServiceMock.Object, _mapper, _employeeServiceMock.Object);

            var command = new UpdateEmployeeCommand
            {
                Id = "employee-id",
                Email = "existing-email@email.com",
                // ... other properties
            };
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.ValidationResult.IsValid);
            Assert.Null(result.Result);
            Assert.Contains(result.ValidationResult.Errors, (x) => x.ErrorMessage.Equals("Email already exists"));
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        [Fact]
        public async Task Handle_FailedToUpdate_ReturnsInternalServerError()
        {
            // Arrange
            _employeeServiceMock.Setup(x => x.UpdateEmployee(It.IsAny<string>(), It.IsAny<EmployeeDto>(), new CancellationToken()))
                .ReturnsAsync((string corrId, EmployeeDto employeeDto, CancellationToken cancellationToken) =>
                    (null, "Update failed", HttpStatusCode.InternalServerError));

            _handler = new UpdateEmployeeCommandHandler(_logger, _validationServiceMock.Object, _mapper, _employeeServiceMock.Object);

            var command = new UpdateEmployeeCommand
            {
                Id = "employee-id",
                Email = "test@email.com",
                // ... other properties
            };
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.ValidationResult.IsValid);
            Assert.Null(result.Result);
            Assert.Contains(result.ValidationResult.Errors, (x) => x.ErrorMessage.Equals("Update failed"));
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        [Fact]
        public async Task Handle_Exception_ReturnsInternalServerError()
        {
            // Arrange
            _employeeServiceMock.Setup(x => x.UpdateEmployee(It.IsAny<string>(), It.IsAny<EmployeeDto>(), new CancellationToken()))
                .ReturnsAsync((string corrId, EmployeeDto employeeDto, CancellationToken cancellationToken) =>
                    throw new Exception("Force exception"));

            _handler = new UpdateEmployeeCommandHandler(_logger, _validationServiceMock.Object, _mapper, _employeeServiceMock.Object);

            var command = new UpdateEmployeeCommand
            {
                Id = "employee-id",
                Email = "test@email.com",
                // ... other properties
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
            obj.Setup(x => x.ValidateAsync<UpdateEmployeeCommand>(It.IsAny<UpdateEmployeeCommand>()))
                .ReturnsAsync((UpdateEmployeeCommand command) =>
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
                .Setup(x => x.Map<UpdateEmployeeCommand, EmployeeDto>(It.IsAny<UpdateEmployeeCommand>()))
                .Returns((UpdateEmployeeCommand command) => new EmployeeDto()
                {
                    Email = command.Email,
                    // ... other properties
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
                .Setup(x => x.UpdateEmployee(It.IsAny<string>(), It.IsAny<EmployeeDto>(), new CancellationToken()))
                .ReturnsAsync((string corrId, EmployeeDto employeeDto, CancellationToken cancellationToken) =>
                    (employeeDto, "Update successful", HttpStatusCode.OK));

            return obj;
        }
    }
}
