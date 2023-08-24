using Dtos;
using Dtos.Enums;
using Dtos.Responses;
using Entities.Enums;
using MediatR;

namespace Domain.Queries;

public class GetEmployeesQuery : BaseQuery, IRequest<QueryResponse<List<EmployeeDto>>>
{
    public string? Name { get; set; }
    public Departments? Department { get; set; }
    public string? Email { get; set; }
    
    public string? SortBy { get; set; }

    public SortTypeEnum SortType { get; set; } = SortTypeEnum.ASC;
}