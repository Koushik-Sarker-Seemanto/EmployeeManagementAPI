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
    private readonly ApplicationDbContext _dbContext;

    
    public CreateEmployeeCommandHandler(ILogger<CreateEmployeeCommandHandler> logger, IValidationService validationService, IMapper mapper, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _validationService = validationService;
        _mapper = mapper;
        _dbContext = dbContext;
    }
    
    public async Task<CommandResponse> Handle(CreateEmployeeCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"CreateEmployeeCommandHandler STARTED with CorrelationId: {command.CorrelationId}");
        CommandResponse response = new CommandResponse
        {
            ValidationResult = new ValidationResponse(),
        };
        try
        {
            ValidationResponse validationResponse = await _validationService.ValidateAsync(command);
            if (!validationResponse.IsValid)
            {
                _logger.LogError($"CreateEmployeeCommandHandler -> Validation error occurred for CorrelationId: {command.CorrelationId}");
                _logger.LogInformation($"CreateEmployeeCommandHandler ENDED with failure for CorrelationId: {command.CorrelationId}");
                response.ValidationResult = validationResponse;
                return response;
            }

            EmployeeDto employeeDto = _mapper.Map<CreateEmployeeCommand, EmployeeDto>(command);
            Employee employee = _mapper.Map<EmployeeDto, Employee>(employeeDto);
            employee.Id = Guid.NewGuid().ToString();
            var res = await _dbContext.Employees.AddAsync(employee, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            response.Result = res.Entity;
            _logger.LogInformation($"CreateEmployeeCommandHandler ENDED Successfully with CorrelationId: {command.CorrelationId}");
            return response;
        }
        catch (Exception e)
        {
            _logger.LogError($"Exception occurred while executing CreateEmployeeCommandHandler for CorrelationId: {command.CorrelationId}, Error: {e.Message}");
            _logger.LogInformation($"CreateEmployeeCommandHandler ENDED with failure for CorrelationId: {command.CorrelationId}");
            response.ValidationResult.AddError(e.Message);
            return response;
        }
    }
}