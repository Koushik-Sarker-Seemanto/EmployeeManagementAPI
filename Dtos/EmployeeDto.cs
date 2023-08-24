using Entities.Enums;

namespace Dtos;

public class EmployeeDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime DoB { get; set; }
    public Departments Department { get; set; }
}