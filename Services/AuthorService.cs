using Microsoft.EntityFrameworkCore;
using LibraryManagement.Api.Data;
using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Models;

namespace LibraryManagement.Api.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly LibraryContext _context;
        
        public AuthorService(LibraryContext context)
        {
            _context = context;
        }
        
        public async Task<PagedResponse<AuthorDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            try
            {
                var query = _context.Authors.AsQueryable();
                
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(a => a.FirstName.Contains(searchTerm) || 
                                           a.LastName.Contains(searchTerm) ||
                                           (a.Biography != null && a.Biography.Contains(searchTerm)) ||
                                           (a.Nationality != null && a.Nationality.Contains(searchTerm)));
                }
                
                var totalCount = await query.CountAsync();
                
                var authors = await query
                    .Include(a => a.Books)
                    .OrderBy(a => a.FirstName)
                    .ThenBy(a => a.LastName)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(a => new AuthorDto
                    {
                        Id = a.Id,
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        FullName = a.FirstName + " " + a.LastName,
                        Biography = a.Biography,
                        BirthDate = a.BirthDate,
                        DeathDate = a.DeathDate,
                        Nationality = a.Nationality,
                        CreatedAt = a.CreatedAt,
                        UpdatedAt = a.UpdatedAt,
                        BooksCount = a.Books.Count
                    })
                    .ToListAsync();
                
                return PagedResponse<AuthorDto>.Create(authors, totalCount, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                return new PagedResponse<AuthorDto>
                {
                    Success = false,
                    Message = "Erro ao recuperar autores",
                    Data = new List<AuthorDto>()
                };
            }
        }
        
        public async Task<ApiResponse<AuthorDto>> GetByIdAsync(int id)
        {
            try
            {
                var author = await _context.Authors
                    .Include(a => a.Books)
                    .FirstOrDefaultAsync(a => a.Id == id);
                
                if (author == null)
                {
                    return ApiResponse<AuthorDto>.ErrorResponse("Autor não encontrado");
                }
                
                var authorDto = new AuthorDto
                {
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName,
                    FullName = author.FullName,
                    Biography = author.Biography,
                    BirthDate = author.BirthDate,
                    DeathDate = author.DeathDate,
                    Nationality = author.Nationality,
                    CreatedAt = author.CreatedAt,
                    UpdatedAt = author.UpdatedAt,
                    BooksCount = author.Books.Count
                };
                
                return ApiResponse<AuthorDto>.SuccessResponse(authorDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<AuthorDto>.ErrorResponse("Erro ao recuperar autor");
            }
        }
        
        public async Task<ApiResponse<AuthorDto>> CreateAsync(CreateAuthorDto createAuthorDto)
        {
            try
            {
                // Check if author with same name already exists
                var existingAuthor = await _context.Authors
                    .FirstOrDefaultAsync(a => a.FirstName.ToLower() == createAuthorDto.FirstName.ToLower() && 
                                            a.LastName.ToLower() == createAuthorDto.LastName.ToLower());
                
                if (existingAuthor != null)
                {
                    return ApiResponse<AuthorDto>.ErrorResponse("Já existe um autor com este nome");
                }
                
                var author = new Author
                {
                    FirstName = createAuthorDto.FirstName,
                    LastName = createAuthorDto.LastName,
                    Biography = createAuthorDto.Biography,
                    BirthDate = createAuthorDto.BirthDate,
                    DeathDate = createAuthorDto.DeathDate,
                    Nationality = createAuthorDto.Nationality
                };
                
                _context.Authors.Add(author);
                await _context.SaveChangesAsync();
                
                var authorDto = new AuthorDto
                {
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName,
                    FullName = author.FullName,
                    Biography = author.Biography,
                    BirthDate = author.BirthDate,
                    DeathDate = author.DeathDate,
                    Nationality = author.Nationality,
                    CreatedAt = author.CreatedAt,
                    UpdatedAt = author.UpdatedAt,
                    BooksCount = 0
                };
                
                return ApiResponse<AuthorDto>.SuccessResponse(authorDto, "Autor criado com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<AuthorDto>.ErrorResponse("Erro ao criar autor");
            }
        }
        
        public async Task<ApiResponse<AuthorDto>> UpdateAsync(int id, UpdateAuthorDto updateAuthorDto)
        {
            try
            {
                var author = await _context.Authors.FindAsync(id);
                
                if (author == null)
                {
                    return ApiResponse<AuthorDto>.ErrorResponse("Autor não encontrado");
                }
                
                // Check if another author with same name already exists
                var existingAuthor = await _context.Authors
                    .FirstOrDefaultAsync(a => a.FirstName.ToLower() == updateAuthorDto.FirstName.ToLower() && 
                                            a.LastName.ToLower() == updateAuthorDto.LastName.ToLower() && 
                                            a.Id != id);
                
                if (existingAuthor != null)
                {
                    return ApiResponse<AuthorDto>.ErrorResponse("Já existe outro autor com este nome");
                }
                
                author.FirstName = updateAuthorDto.FirstName;
                author.LastName = updateAuthorDto.LastName;
                author.Biography = updateAuthorDto.Biography;
                author.BirthDate = updateAuthorDto.BirthDate;
                author.DeathDate = updateAuthorDto.DeathDate;
                author.Nationality = updateAuthorDto.Nationality;
                
                await _context.SaveChangesAsync();
                
                var authorDto = new AuthorDto
                {
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName,
                    FullName = author.FullName,
                    Biography = author.Biography,
                    BirthDate = author.BirthDate,
                    DeathDate = author.DeathDate,
                    Nationality = author.Nationality,
                    CreatedAt = author.CreatedAt,
                    UpdatedAt = author.UpdatedAt,
                    BooksCount = await _context.Books.CountAsync(b => b.AuthorId == author.Id)
                };
                
                return ApiResponse<AuthorDto>.SuccessResponse(authorDto, "Autor atualizado com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<AuthorDto>.ErrorResponse("Erro ao atualizar autor");
            }
        }
        
        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var author = await _context.Authors
                    .Include(a => a.Books)
                    .FirstOrDefaultAsync(a => a.Id == id);
                
                if (author == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Autor não encontrado");
                }
                
                if (author.Books.Any())
                {
                    return ApiResponse<bool>.ErrorResponse("Não é possível excluir um autor que possui livros associados");
                }
                
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
                
                return ApiResponse<bool>.SuccessResponse(true, "Autor excluído com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse("Erro ao excluir autor");
            }
        }
        
        public async Task<ApiResponse<bool>> ExistsAsync(int id)
        {
            try
            {
                var exists = await _context.Authors.AnyAsync(a => a.Id == id);
                return ApiResponse<bool>.SuccessResponse(exists);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse("Erro ao verificar existência do autor");
            }
        }
    }
}

