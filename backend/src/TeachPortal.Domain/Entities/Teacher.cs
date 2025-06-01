using System.ComponentModel.DataAnnotations;

namespace TeachPortal.Domain.Entities;

public class Teacher : BaseEntity
{
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    
    // Computed property
    public string FullName => $"{FirstName} {LastName}";
    
    public int StudentCount => Students?.Count ?? 0;
} 