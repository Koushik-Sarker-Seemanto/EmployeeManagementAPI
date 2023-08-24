using System.ComponentModel.DataAnnotations;

namespace Entities;

public class BaseEntity
{
    [Key]
    public string Id { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}