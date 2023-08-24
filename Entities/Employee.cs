using System.ComponentModel.DataAnnotations;
using Entities.Enums;

namespace Entities;

public class Employee : BaseEntity
{
    public Employee()
    {
        this.CreatedAt = DateTime.UtcNow;
        this.UpdatedAt = DateTime.UtcNow;
    }
    [Required]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public DateTime DoB { get; set; }

    [Required]
    public Departments Department { get; set; }
}