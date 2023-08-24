using Dtos;
using Dtos.Responses;
using MediatR;

namespace Domain.Queries;

public class GetEmployeeByIdQuery : BaseQuery, IRequest<QueryResponse<EmployeeDto>>
{
    public string Id { get; set; }
}