using System.Net;
using AutoMapper;
using Domain.Commands;
using Dtos;
using Dtos.Responses;
using Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Repository;
using Services.Abstraction;

namespace Domain.CommandHandlers;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, CommandResponse>
{
    private readonly ILogger<UpdateEmployeeCommandHandler> _logger;
    private readonly IValidationService _validationService;
    private readonly IMapper _mapper;
    private readonly IEmployeeService _employeeService;
    
    public UpdateEmployeeCommandHandler(ILogger<UpdateEmployeeCommandHandler> logger, IValidationService validationService, IMapper mapper, ApplicationDbContext dbContext, IEmployeeService employeeService)
    {
        _logger = logger;
        _validationService = validationService;
        _mapper = mapper;
        _employeeService = employeeService;
    }
    
    public async Task<CommandResponse> Handle(UpdateEmployeeCommand command, CancellationToken cancellationToken)
    {
        string correlationId = command.CorrelationId ?? Guid.NewGuid().ToString();
        _logger.LogInformation($"UpdateEmployeeCommandHandler STARTED with CorrelationId: {correlationId}");
        CommandResponse response = new CommandResponse
        {
            ValidationResult = new ValidationResponse(),
        };
        try
        {
            ValidationResponse validationResponse = await _validationService.ValidateAsync(command);
            if (!validationResponse.IsValid)
            {
                _logger.LogError($"UpdateEmployeeCommandHandler -> Validation error occurred for CorrelationId: {correlationId}");
                _logger.LogInformation($"UpdateEmployeeCommandHandler ENDED with failure for CorrelationId: {correlationId}");
                response.ValidationResult = validationResponse;
                return response;
            }

            Employee? isEmailValid = await _employeeService.GetEmployeeAsync(e => e.Email == command.Email && e.Id != command.Id, cancellationToken).ConfigureAwait(false);

            if (isEmailValid != null)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ValidationResult.AddError("Email already exists", "Email");
                _logger.LogInformation($"UpdateEmployeeCommandHandler ENDED with failure for CorrelationId: {correlationId}");
                return response;
            }

            EmployeeDto employeeDto = _mapper.Map<UpdateEmployeeCommand, EmployeeDto>(command);
            (EmployeeDto? result, string message, HttpStatusCode status) = await _employeeService.UpdateEmployee(correlationId, employeeDto, cancellationToken);
            if (result != null)
            {
                response.StatusCode = status;
                response.Result = result;
                _logger.LogInformation($"UpdateEmployeeCommandHandler ENDED Successfully with CorrelationId: {correlationId}");
                return response;
            }

            response.StatusCode = status;
            response.ValidationResult.AddError(message);
            _logger.LogInformation($"UpdateEmployeeCommandHandler ENDED with failure for CorrelationId: {correlationId}");
            return response;
        }
        catch (Exception e)
        {
            _logger.LogError($"Exception occurred while executing UpdateEmployeeCommandHandler for CorrelationId: {correlationId}, Error: {e.Message}");
            _logger.LogInformation($"UpdateEmployeeCommandHandler ENDED with failure for CorrelationId: {correlationId}");
            response.ValidationResult.AddError(e.Message);
            response.StatusCode = HttpStatusCode.InternalServerError;
            return response;
        }
    }
}