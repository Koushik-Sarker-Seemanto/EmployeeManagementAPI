using System.Net;
using Domain.Queries;
using Dtos;
using Dtos.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using Services.Abstraction;

namespace Domain.QueryHandlers;

public class DeleteEmployeeQueryHandler : IRequestHandler<DeleteEmployeeQuery, QueryResponse<EmployeeDto>>
{
    private readonly ILogger<DeleteEmployeeQueryHandler> _logger;
    private readonly IEmployeeService _employeeService;
    
    public DeleteEmployeeQueryHandler(ILogger<DeleteEmployeeQueryHandler> logger, IEmployeeService employeeService)
    {
        _logger = logger;
        _employeeService = employeeService;
    }
    
    public async Task<QueryResponse<EmployeeDto>> Handle(DeleteEmployeeQuery query, CancellationToken cancellationToken)
    {
        string correlationId = query.CorrelationId ?? Guid.NewGuid().ToString();
        _logger.LogInformation($"DeleteEmployeeQueryHandler STARTED with CorrelationId: {correlationId}");
        QueryResponse<EmployeeDto> response = new QueryResponse<EmployeeDto>
        {
            ValidationResult = new ValidationResponse(),
        };
        try
        {
            (EmployeeDto? result, string message, HttpStatusCode status) = await _employeeService.DeleteEmployee(correlationId, query.Id, cancellationToken).ConfigureAwait(false);
            if (result != null)
            {
                response.StatusCode = status;
                response.Result = result;
                _logger.LogInformation($"DeleteEmployeeQueryHandler ENDED Successfully with CorrelationId: {correlationId}");
                return response;
            }

            response.StatusCode = status;
            response.ValidationResult.AddError(message);
            _logger.LogInformation($"DeleteEmployeeQueryHandler ENDED with failure for CorrelationId: {correlationId}");
            return response;
        }
        catch (Exception e)
        {
            _logger.LogError($"Exception occurred while executing DeleteEmployeeQueryHandler for CorrelationId: {correlationId}, Error: {e.Message}");
            _logger.LogInformation($"DeleteEmployeeQueryHandler ENDED with failure for CorrelationId: {correlationId}");
            response.ValidationResult.AddError(e.Message);
            response.StatusCode = HttpStatusCode.InternalServerError;
            return response;
        }
    }
}