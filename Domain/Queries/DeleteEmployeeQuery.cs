using Dtos;
using Dtos.Responses;
using MediatR;

namespace Domain.Queries;

public class DeleteEmployeeQuery : BaseQuery, IRequest<QueryResponse<EmployeeDto>>
{
    public string Id { get; set; }
}