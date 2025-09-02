using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.Models
{
    public class Author
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Biography { get; set; }
        
        public DateTime? BirthDate { get; set; }
        
        public DateTime? DeathDate { get; set; }
        
        [StringLength(100)]
        public string? Nationality { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
        
        // Computed property
        public string FullName => $"{FirstName} {LastName}";
    }
}

