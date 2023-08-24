using System.Linq.Expressions;
using System.Net;
using Dtos;
using Entities;

namespace Services.Abstraction;

public interface IEmployeeService
{
    public Task<Employee?> GetEmployeeAsync(Expression<Func<Employee, bool>> expression,
        CancellationToken cancellationToken);

    Task<IQueryable<Employee>> GetEmployeesAsync(Expression<Func<Employee, bool>> expression,
        CancellationToken cancellationToken, IQueryable<Employee>? employees = null);
    
    public Task<EmployeeDto?> CreateEmployee(string correlationId, EmployeeDto employeeDto,
        CancellationToken cancellationToken);

    public Task<(EmployeeDto?, string, HttpStatusCode)> UpdateEmployee(string correlationId, EmployeeDto employeeDto,
        CancellationToken cancellationToken);

    public Task<(EmployeeDto?, string, HttpStatusCode)> DeleteEmployee(string correlationId, string id,
        CancellationToken cancellationToken);
}