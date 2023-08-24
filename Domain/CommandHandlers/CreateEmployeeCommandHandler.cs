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

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, CommandResponse>
{
    private readonly ILogger<CreateEmployeeCommandHandler> _logger;
    private readonly IValidationService _validationService;
    private readonly IMapper _mapper;
    private readonly IEmployeeService _employeeService;
    
    public CreateEmployeeCommandHandler(ILogger<CreateEmployeeCommandHandler> logger, IValidationService validationService, IMapper mapper, ApplicationDbContext dbContext, IEmployeeService employeeService)
    {
        _logger = logger;
        _validationService = validationService;
        _mapper = mapper;
        _employeeService = employeeService;
    }
    
    public async Task<CommandResponse> Handle(CreateEmployeeCommand command, CancellationToken cancellationToken)
    {
        string correlationId = command.CorrelationId ?? Guid.NewGuid().ToString();
        _logger.LogInformation($"CreateEmployeeCommandHandler STARTED with CorrelationId: {correlationId}");
        CommandResponse response = new CommandResponse
        {
            ValidationResult = new ValidationResponse(),
        };
        try
        {
            ValidationResponse validationResponse = await _validationService.ValidateAsync(command);
            if (!validationResponse.IsValid)
            {
                _logger.LogError($"CreateEmployeeCommandHandler -> Validation error occurred for CorrelationId: {correlationId}");
                _logger.LogInformation($"CreateEmployeeCommandHandler ENDED with failure for CorrelationId: {correlationId}");
                response.ValidationResult = validationResponse;
                return response;
            }

            EmployeeDto employeeDto = _mapper.Map<CreateEmployeeCommand, EmployeeDto>(command);
            employeeDto.Id = Guid.NewGuid().ToString();
            
            Employee? isEmailAvailable = await _employeeService.GetEmployeeAsync(correlationId,
                e => e.Email == command.Email, cancellationToken);
            
            if (isEmailAvailable != null)
            {
                response.ValidationResult.AddError("Email already exists", "Email");
                _logger.LogInformation($"CreateEmployeeCommandHandler ENDED with failure for CorrelationId: {correlationId}");
                return response;
            }

            EmployeeDto? result = await _employeeService.CreateEmployee(correlationId, employeeDto, cancellationToken);
            if (result != null)
            {
                response.Result = result;
                _logger.LogInformation($"CreateEmployeeCommandHandler ENDED Successfully with CorrelationId: {correlationId}");
                return response;
            }
            response.ValidationResult.AddError("Failed to create employee");
            _logger.LogInformation($"CreateEmployeeCommandHandler ENDED with failure for CorrelationId: {correlationId}");
            return response;
        }
        catch (Exception e)
        {
            _logger.LogError($"Exception occurred while executing CreateEmployeeCommandHandler for CorrelationId: {correlationId}, Error: {e.Message}");
            _logger.LogInformation($"CreateEmployeeCommandHandler ENDED with failure for CorrelationId: {correlationId}");
            response.ValidationResult.AddError(e.Message);
            response.StatusCode = HttpStatusCode.InternalServerError;
            return response;
        }
    }
}