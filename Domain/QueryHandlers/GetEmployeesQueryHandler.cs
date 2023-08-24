using System.Net;
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

public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, QueryResponse<List<EmployeeDto>>>
{
    private readonly ILogger<GetEmployeesQueryHandler> _logger;
    private readonly IEmployeeService _employeeService;
    private readonly IMapper _mapper;
    private readonly IValidationService _validationService;
    
    public GetEmployeesQueryHandler(ILogger<GetEmployeesQueryHandler> logger, IEmployeeService employeeService, IMapper mapper, IValidationService validationService)
    {
        _logger = logger;
        _employeeService = employeeService;
        _mapper = mapper;
        _validationService = validationService;
    }

    public async Task<QueryResponse<List<EmployeeDto>>> Handle(GetEmployeesQuery query,
        CancellationToken cancellationToken)
    {
        string correlationId = query.CorrelationId ?? Guid.NewGuid().ToString();
        _logger.LogInformation($"GetEmployeesQueryHandler STARTED with CorrelationId: {correlationId}");
        QueryResponse<List<EmployeeDto>> response = new QueryResponse<List<EmployeeDto>>
        {
            ValidationResult = new ValidationResponse(),
        };
        try
        {
            ValidationResponse validationResult = await _validationService.ValidateAsync(query);
            if (!validationResult.IsValid)
            {
                _logger.LogError($"GetEmployeesQueryHandler -> Validation error occurred for CorrelationId: {correlationId}");
                _logger.LogInformation($"GetEmployeesQueryHandler ENDED with failure for CorrelationId: {correlationId}");
                response.ValidationResult = validationResult;
                return response;
            }
            
            IQueryable<Employee>? employees = await BuildQueryable(query, cancellationToken, correlationId);

            employees = BuildOrderByQyQueryable(query, employees);
            

            List<Employee> employeeList = await employees?.Skip(query.PageSize * query.PageNo)
                ?.Take(query.PageSize)?.ToListAsync()!;
            List<EmployeeDto>? employeeDtoList =
                employeeList?.Select(x => _mapper.Map<Employee, EmployeeDto>(x))?.ToList();
            response.Result = employeeDtoList!;
            return response;
        }
        catch (Exception e)
        {
            _logger.LogError(
                $"Exception occurred while executing GetEmployeesQueryHandler for CorrelationId: {correlationId}, Error: {e.Message}");
            _logger.LogInformation($"GetEmployeesQueryHandler ENDED with failure for CorrelationId: {correlationId}");
            response.ValidationResult.AddError(e.Message);
            response.StatusCode = HttpStatusCode.InternalServerError;
            return response;
        }

    }

    private async Task<IQueryable<Employee>> BuildQueryable(GetEmployeesQuery query, CancellationToken cancellationToken, string correlationId)
    {
        IQueryable<Employee>? employees = null;
        if (query.Name != null)
        {
            _logger.LogInformation(
                $"GetEmployeesQueryHandler -> Going to apply condition on Name: {query.Name} for CorrelationId: {correlationId}");
            employees = await _employeeService
                .GetEmployeesAsync(e => e.Name.Contains(query.Name), cancellationToken).ConfigureAwait(false);
        }

        if (query.Email != null)
        {
            _logger.LogInformation(
                $"GetEmployeesQueryHandler -> Going to apply condition on Email: {query.Email} for CorrelationId: {correlationId}");
            employees = await _employeeService
                .GetEmployeesAsync(e => e.Name.Contains(query.Email), cancellationToken, employees)
                .ConfigureAwait(false);
        }

        if (query.Department != null)
        {
            _logger.LogInformation(
                $"GetEmployeesQueryHandler -> Going to apply condition on Department: {query.Department} for CorrelationId: {correlationId}");
            employees = await _employeeService.GetEmployeesAsync(e => e.Department == query.Department,
                cancellationToken, employees).ConfigureAwait(false);
        }

        if (employees == null)
        {
            _logger.LogInformation(
                $"GetEmployeesQueryHandler -> Going to fetch all Employees for CorrelationId: {correlationId}");
            employees = await _employeeService.GetEmployeesAsync(e => true, cancellationToken)
                .ConfigureAwait(false);
        }

        return employees;
    }

    private static IQueryable<Employee> BuildOrderByQyQueryable(GetEmployeesQuery query, IQueryable<Employee> employees)
    {
        if (query.SortBy != null)
        {
            if (query.SortType == SortTypeEnum.ASC)
            {
                employees = query.SortBy.ToLower() switch
                {
                    "department" => employees.OrderBy(e => e.Department),
                    "email" => employees.OrderBy(e => e.Email),
                    "dob" => employees.OrderBy(e => e.DoB),
                    _ => employees.OrderBy(e => e.Name)
                };
            }
            else
            {
                employees = query.SortBy.ToLower() switch
                {
                    "department" => employees.OrderByDescending(e => e.Department),
                    "email" => employees.OrderByDescending(e => e.Email),
                    "dob" => employees.OrderByDescending(e => e.DoB),
                    _ => employees.OrderByDescending(e => e.Name)
                };
            }
        }
        else
        {
            employees = query.SortType == SortTypeEnum.ASC ? employees.OrderBy(e => e.Name) : employees.OrderByDescending(e => e.Name);
        }

        return employees;
    }
}
