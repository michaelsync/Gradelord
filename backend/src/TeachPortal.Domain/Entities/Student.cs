using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeachPortal.Domain.Entities;

public class Student : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public Guid TeacherId { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(TeacherId))]
    public virtual Teacher Teacher { get; set; } = null!;
    
    // Computed property
    public string FullName => $"{FirstName} {LastName}";
} 