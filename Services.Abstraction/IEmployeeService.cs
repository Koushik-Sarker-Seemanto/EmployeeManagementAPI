using System.Linq.Expressions;
using Dtos;
using Entities;

namespace Services.Abstraction;

public interface IEmployeeService
{
    public Task<Employee?> GetEmployeeAsync(string correlationId, Expression<Func<Employee, bool>> expression,
        CancellationToken cancellationToken);

    Task<IQueryable<Employee>> GetEmployeesAsync(string correlationId, Expression<Func<Employee, bool>> expression,
        CancellationToken cancellationToken);
    
    public Task<EmployeeDto?> CreateEmployee(string correlationId, EmployeeDto employeeDto,
        CancellationToken cancellationToken);

    public Task<(EmployeeDto?, string)> UpdateEmployee(string correlationId, EmployeeDto employeeDto,
        CancellationToken cancellationToken);
}