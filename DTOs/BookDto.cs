using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ISBN { get; set; }
        public string? Description { get; set; }
        public DateTime? PublicationDate { get; set; }
        public string? Publisher { get; set; }
        public int? Pages { get; set; }
        public decimal? Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Author information
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        
        // Genre information
        public int GenreId { get; set; }
        public string GenreName { get; set; } = string.Empty;
    }
    
    public class CreateBookDto
    {
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres")]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(20, ErrorMessage = "O ISBN deve ter no máximo 20 caracteres")]
        public string? ISBN { get; set; }
        
        [StringLength(2000, ErrorMessage = "A descrição deve ter no máximo 2000 caracteres")]
        public string? Description { get; set; }
        
        public DateTime? PublicationDate { get; set; }
        
        [StringLength(100, ErrorMessage = "A editora deve ter no máximo 100 caracteres")]
        public string? Publisher { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "O número de páginas deve ser maior que zero")]
        public int? Pages { get; set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
        public decimal? Price { get; set; }
        
        [Required(ErrorMessage = "O autor é obrigatório")]
        public int AuthorId { get; set; }
        
        [Required(ErrorMessage = "O gênero é obrigatório")]
        public int GenreId { get; set; }
    }
    
    public class UpdateBookDto
    {
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres")]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(20, ErrorMessage = "O ISBN deve ter no máximo 20 caracteres")]
        public string? ISBN { get; set; }
        
        [StringLength(2000, ErrorMessage = "A descrição deve ter no máximo 2000 caracteres")]
        public string? Description { get; set; }
        
        public DateTime? PublicationDate { get; set; }
        
        [StringLength(100, ErrorMessage = "A editora deve ter no máximo 100 caracteres")]
        public string? Publisher { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "O número de páginas deve ser maior que zero")]
        public int? Pages { get; set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
        public decimal? Price { get; set; }
        
        [Required(ErrorMessage = "O autor é obrigatório")]
        public int AuthorId { get; set; }
        
        [Required(ErrorMessage = "O gênero é obrigatório")]
        public int GenreId { get; set; }
    }
}

