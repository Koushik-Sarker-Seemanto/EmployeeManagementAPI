using Dtos.Responses;
using Entities.Enums;
using MediatR;

namespace Domain.Commands;

public class CreateEmployeeCommand : BaseCommand, IRequest<CommandResponse>
{
    public string Name { get; set; }
    public string Email { get; set; }

    public DateTime DoB { get; set; }

    public Departments Department { get; set; }
}