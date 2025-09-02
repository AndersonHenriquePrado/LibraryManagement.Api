using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.DTOs
{
    public class AuthorDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Biography { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public string? Nationality { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int BooksCount { get; set; }
    }
    
    public class CreateAuthorDto
    {
        [Required(ErrorMessage = "O primeiro nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O primeiro nome deve ter no máximo 100 caracteres")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "O sobrenome é obrigatório")]
        [StringLength(100, ErrorMessage = "O sobrenome deve ter no máximo 100 caracteres")]
        public string LastName { get; set; } = string.Empty;
        
        [StringLength(1000, ErrorMessage = "A biografia deve ter no máximo 1000 caracteres")]
        public string? Biography { get; set; }
        
        public DateTime? BirthDate { get; set; }
        
        public DateTime? DeathDate { get; set; }
        
        [StringLength(100, ErrorMessage = "A nacionalidade deve ter no máximo 100 caracteres")]
        public string? Nationality { get; set; }
    }
    
    public class UpdateAuthorDto
    {
        [Required(ErrorMessage = "O primeiro nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O primeiro nome deve ter no máximo 100 caracteres")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "O sobrenome é obrigatório")]
        [StringLength(100, ErrorMessage = "O sobrenome deve ter no máximo 100 caracteres")]
        public string LastName { get; set; } = string.Empty;
        
        [StringLength(1000, ErrorMessage = "A biografia deve ter no máximo 1000 caracteres")]
        public string? Biography { get; set; }
        
        public DateTime? BirthDate { get; set; }
        
        public DateTime? DeathDate { get; set; }
        
        [StringLength(100, ErrorMessage = "A nacionalidade deve ter no máximo 100 caracteres")]
        public string? Nationality { get; set; }
    }
}

