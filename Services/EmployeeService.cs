using System.Linq.Expressions;
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

    public async Task<Employee?> GetEmployeeAsync(string correlationId, Expression<Func<Employee, bool>> expression,
        CancellationToken cancellationToken)
    {
        Employee? employee = await _dbContext.Employees.FirstOrDefaultAsync(expression, cancellationToken);
        return employee;
    }
    
    public async Task<IQueryable<Employee>> GetEmployeesAsync(string correlationId, Expression<Func<Employee, bool>> expression,
        CancellationToken cancellationToken)
    {
        var employee = _dbContext.Employees.Where(expression);
        return employee;
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
    
    public async Task<(EmployeeDto?, string)> UpdateEmployee(string correlationId, EmployeeDto employeeDto, CancellationToken cancellationToken)
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
                employee.DoB = employee.DoB;
                employee.UpdatedAt = DateTime.UtcNow;
                var response =  _dbContext.Employees.Update(employee);
                await _dbContext.SaveChangesAsync(cancellationToken);
                EmployeeDto result = _mapper.Map<Employee, EmployeeDto>(response.Entity);
                _logger.LogInformation($"UpdateEmployee ENDED Successfully with CorrelationId: {correlationId}");
                return (result, "");
            }

            _logger.LogInformation($"UpdateEmployee ENDED with Failure for CorrelationId: {correlationId}");
            return (null, "Employee Id not found");
        }
        catch (Exception e)
        {
            _logger.LogError($"Exception occurred while executing UpdateEmployee for CorrelationId: {correlationId}, Error: {e.Message}");
            return (null, e.Message);
        }
    }

    public async Task<bool> IsEmailAvailable(string correlationId, string email,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"IsEmailAvailable STARTED with CorrelationId: {correlationId}");
        try
        {
            Employee? employee = await  _dbContext.Employees.FirstOrDefaultAsync(emp => emp.Email.Contains(email), cancellationToken).ConfigureAwait(false);
            _logger.LogInformation($"IsEmailAvailable ENDED Successfully isAvailable: {employee == null} for CorrelationId: {correlationId}");
            return employee == null;
        }
        catch (Exception e)
        {
            _logger.LogError($"Exception occurred while executing IsEmailAvailable for CorrelationId: {correlationId}, Error: {e.Message}");
            return false;
        }
    }

    public async Task<EmployeeDto?> GetEmployeeByEmail(string correlationId, string email, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"GetEmployeeByEmail STARTED with CorrelationId: {correlationId}");
        try
        {
            Employee? employee = await  _dbContext.Employees.FirstOrDefaultAsync(emp => emp.Email.Contains(email), cancellationToken).ConfigureAwait(false);
            if (employee != null)
            {
                EmployeeDto employeeDto = _mapper.Map<Employee, EmployeeDto>(employee);
                _logger.LogInformation($"GetEmployeeByEmail ENDED Successfully with CorrelationId: {correlationId}");
                return employeeDto;
            }

            _logger.LogInformation($"GetEmployeeByEmail ENDED with Failure for CorrelationId: {correlationId}");
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError($"Exception occurred while executing GetEmployeeByEmail for CorrelationId: {correlationId}, Error: {e.Message}");
            return null;
        }
    }
}