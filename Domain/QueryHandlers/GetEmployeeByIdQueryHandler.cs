using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using AutoMapper;
using Domain.Queries;
using Dtos;
using Dtos.Enums;
using Dtos.Responses;
using Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Abstraction;

namespace Domain.QueryHandlers;

public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, QueryResponse<EmployeeDto>>
{
    private readonly ILogger<GetEmployeeByIdQueryHandler> _logger;
    private readonly IEmployeeService _employeeService;
    private readonly IMapper _mapper;
    private readonly IValidationService _validationService;
    
    public GetEmployeeByIdQueryHandler(ILogger<GetEmployeeByIdQueryHandler> logger, IEmployeeService employeeService, IMapper mapper, IValidationService validationService)
    {
        _logger = logger;
        _employeeService = employeeService;
        _mapper = mapper;
        _validationService = validationService;
    }

    public async Task<QueryResponse<EmployeeDto>> Handle(GetEmployeeByIdQuery query,
        CancellationToken cancellationToken)
    {
        string correlationId = query.CorrelationId ?? Guid.NewGuid().ToString();
        _logger.LogInformation($"GetEmployeeByIdQueryHandler STARTED with CorrelationId: {correlationId}");
        QueryResponse<EmployeeDto> response = new QueryResponse<EmployeeDto>
        {
            ValidationResult = new ValidationResponse(),
        };
        try
        {
            ValidationResponse validationResult = await _validationService.ValidateAsync(query);
            if (!validationResult.IsValid)
            {
                _logger.LogError($"GetEmployeeByIdQueryHandler -> Validation error occurred for CorrelationId: {correlationId}");
                _logger.LogInformation($"GetEmployeeByIdQueryHandler ENDED with failure for CorrelationId: {correlationId}");
                response.ValidationResult = validationResult;
                return response;
            }
            
            Employee? employee = await _employeeService.GetEmployeeAsync(x => x.Id == query.Id, cancellationToken).ConfigureAwait(false);
            if (employee == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.ValidationResult.AddError("No employee found with this id", "Id");
                return response;
            }

            EmployeeDto employeeDto = _mapper.Map<Employee, EmployeeDto>(employee);
            response.StatusCode = HttpStatusCode.OK;
            response.Result = employeeDto;
            response.Count = 1;
            return response;
        }
        catch (Exception e)
        {
            _logger.LogError(
                $"Exception occurred while executing GetEmployeeByIdQueryHandler for CorrelationId: {correlationId}, Error: {e.Message}");
            _logger.LogInformation($"GetEmployeeByIdQueryHandler ENDED with failure for CorrelationId: {correlationId}");
            response.ValidationResult.AddError(e.Message);
            response.StatusCode = HttpStatusCode.InternalServerError;
            return response;
        }

    }
}
