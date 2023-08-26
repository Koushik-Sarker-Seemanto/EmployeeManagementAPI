using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using Dtos;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository;
using Services.Abstraction;

namespace Services;

public class EmployeeService : IEmployeeService
{
    private readonly IMapper _mapper;
    private readonly ILogger<EmployeeService> _logger;
    private readonly ApplicationDbContext _dbContext;
    public EmployeeService(ILogger<EmployeeService> logger, ApplicationDbContext dbContext, IMapper mapper)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Employee?> GetEmployeeAsync(Expression<Func<Employee, bool>> expression,
        CancellationToken cancellationToken)
    {
        Employee? employee = await _dbContext.Employees.FirstOrDefaultAsync(expression, cancellationToken);
        return employee;
    }
    
    public async Task<IQueryable<Employee>> GetEmployeesAsync(Expression<Func<Employee, bool>> expression,
        CancellationToken cancellationToken, IQueryable<Employee>? employees)
    {
        if (employees != null)
        {
            var employee = employees.Where(expression);
            return employee;
        }
        else
        {
            var employee = _dbContext.Employees.Where(expression);
            return employee;
        }
    }
    
    public async Task<EmployeeDto?> CreateEmployee(string correlationId, EmployeeDto employeeDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"CreateEmployee STARTED with CorrelationId: {correlationId}");
        try
        {
            Employee employee = _mapper.Map<EmployeeDto, Employee>(employeeDto);
            var response = await _dbContext.Employees.AddAsync(employee, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            EmployeeDto result = _mapper.Map<Employee, EmployeeDto>(response.Entity);
            _logger.LogInformation($"CreateEmployee ENDED Successfully with CorrelationId: {correlationId}");
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError($"Exception occurred while executing CreateEmployee for CorrelationId: {correlationId}, Error: {e.Message}");
            return null;
        }
    }
    
    public async Task<(EmployeeDto?, string, HttpStatusCode)> UpdateEmployee(string correlationId, EmployeeDto employeeDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"UpdateEmployee STARTED with CorrelationId: {correlationId}");
        try
        {
            Employee? employee =
                await _dbContext.Employees.FirstOrDefaultAsync(x => x.Id == employeeDto.Id, cancellationToken).ConfigureAwait(false);
            if (employee != null)
            {
                employee.Email = employeeDto.Email;
                employee.Name = employeeDto.Name;
                employee.Department = employeeDto.Department;
                employee.DoB = employeeDto.DoB;
                employee.UpdatedAt = DateTime.UtcNow;
                var response =  _dbContext.Employees.Update(employee);
                await _dbContext.SaveChangesAsync(cancellationToken);
                EmployeeDto result = _mapper.Map<Employee, EmployeeDto>(response.Entity);
                _logger.LogInformation($"UpdateEmployee ENDED Successfully with CorrelationId: {correlationId}");
                return (result, "", HttpStatusCode.OK);
            }

            _logger.LogInformation($"UpdateEmployee ENDED with Failure for CorrelationId: {correlationId}");
            return (null, "Employee Id not found", HttpStatusCode.BadRequest);
        }
        catch (Exception e)
        {
            _logger.LogError($"Exception occurred while executing UpdateEmployee for CorrelationId: {correlationId}, Error: {e.Message}");
            return (null, e.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<(EmployeeDto?, string, HttpStatusCode)> DeleteEmployee(string correlationId, string id, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"DeleteEmployee STARTED with CorrelationId: {correlationId}");
        try
        {
            Employee? employee =
                await _dbContext.Employees.FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);
            if (employee != null)
            {
                var response = _dbContext.Employees.Remove(employee);
                await _dbContext.SaveChangesAsync(cancellationToken);
                EmployeeDto result = _mapper.Map<Employee, EmployeeDto>(response.Entity);
                _logger.LogInformation($"DeleteEmployee ENDED Successfully with CorrelationId: {correlationId}");
                return (result, "", HttpStatusCode.OK);
            }

            _logger.LogInformation($"DeleteEmployee ENDED with Failure for CorrelationId: {correlationId}");
            return (null, "Employee Id not found", HttpStatusCode.BadRequest);
        }
        catch (Exception e)
        {
            _logger.LogError($"Exception occurred while executing DeleteEmployee for CorrelationId: {correlationId}, Error: {e.Message}");
            return (null, e.Message, HttpStatusCode.InternalServerError);
        }
    }
}