using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Api.Models
{
    public class Book
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string? ISBN { get; set; }
        
        [StringLength(2000)]
        public string? Description { get; set; }
        
        public DateTime? PublicationDate { get; set; }
        
        [StringLength(100)]
        public string? Publisher { get; set; }
        
        public int? Pages { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal? Price { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Foreign Keys
        [Required]
        public int AuthorId { get; set; }
        
        [Required]
        public int GenreId { get; set; }
        
        // Navigation properties
        public virtual Author Author { get; set; } = null!;
        public virtual Genre Genre { get; set; } = null!;
    }
}

