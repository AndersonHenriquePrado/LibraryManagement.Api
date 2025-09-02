using Microsoft.EntityFrameworkCore;
using LibraryManagement.Api.Data;
using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Models;

namespace LibraryManagement.Api.Services
{
    public class GenreService : IGenreService
    {
        private readonly LibraryContext _context;
        
        public GenreService(LibraryContext context)
        {
            _context = context;
        }
        
        public async Task<PagedResponse<GenreDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            try
            {
                var query = _context.Genres.AsQueryable();
                
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(g => g.Name.Contains(searchTerm) || 
                                           (g.Description != null && g.Description.Contains(searchTerm)));
                }
                
                var totalCount = await query.CountAsync();
                
                var genres = await query
                    .Include(g => g.Books)
                    .OrderBy(g => g.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(g => new GenreDto
                    {
                        Id = g.Id,
                        Name = g.Name,
                        Description = g.Description,
                        CreatedAt = g.CreatedAt,
                        UpdatedAt = g.UpdatedAt,
                        BooksCount = g.Books.Count
                    })
                    .ToListAsync();
                
                return PagedResponse<GenreDto>.Create(genres, totalCount, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                return new PagedResponse<GenreDto>
                {
                    Success = false,
                    Message = "Erro ao recuperar gêneros",
                    Data = new List<GenreDto>()
                };
            }
        }
        
        public async Task<ApiResponse<GenreDto>> GetByIdAsync(int id)
        {
            try
            {
                var genre = await _context.Genres
                    .Include(g => g.Books)
                    .FirstOrDefaultAsync(g => g.Id == id);
                
                if (genre == null)
                {
                    return ApiResponse<GenreDto>.ErrorResponse("Gênero não encontrado");
                }
                
                var genreDto = new GenreDto
                {
                    Id = genre.Id,
                    Name = genre.Name,
                    Description = genre.Description,
                    CreatedAt = genre.CreatedAt,
                    UpdatedAt = genre.UpdatedAt,
                    BooksCount = genre.Books.Count
                };
                
                return ApiResponse<GenreDto>.SuccessResponse(genreDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<GenreDto>.ErrorResponse("Erro ao recuperar gênero");
            }
        }
        
        public async Task<ApiResponse<GenreDto>> CreateAsync(CreateGenreDto createGenreDto)
        {
            try
            {
                // Check if genre with same name already exists
                var existingGenre = await _context.Genres
                    .FirstOrDefaultAsync(g => g.Name.ToLower() == createGenreDto.Name.ToLower());
                
                if (existingGenre != null)
                {
                    return ApiResponse<GenreDto>.ErrorResponse("Já existe um gênero com este nome");
                }
                
                var genre = new Genre
                {
                    Name = createGenreDto.Name,
                    Description = createGenreDto.Description
                };
                
                _context.Genres.Add(genre);
                await _context.SaveChangesAsync();
                
                var genreDto = new GenreDto
                {
                    Id = genre.Id,
                    Name = genre.Name,
                    Description = genre.Description,
                    CreatedAt = genre.CreatedAt,
                    UpdatedAt = genre.UpdatedAt,
                    BooksCount = 0
                };
                
                return ApiResponse<GenreDto>.SuccessResponse(genreDto, "Gênero criado com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<GenreDto>.ErrorResponse("Erro ao criar gênero");
            }
        }
        
        public async Task<ApiResponse<GenreDto>> UpdateAsync(int id, UpdateGenreDto updateGenreDto)
        {
            try
            {
                var genre = await _context.Genres.FindAsync(id);
                
                if (genre == null)
                {
                    return ApiResponse<GenreDto>.ErrorResponse("Gênero não encontrado");
                }
                
                // Check if another genre with same name already exists
                var existingGenre = await _context.Genres
                    .FirstOrDefaultAsync(g => g.Name.ToLower() == updateGenreDto.Name.ToLower() && g.Id != id);
                
                if (existingGenre != null)
                {
                    return ApiResponse<GenreDto>.ErrorResponse("Já existe outro gênero com este nome");
                }
                
                genre.Name = updateGenreDto.Name;
                genre.Description = updateGenreDto.Description;
                
                await _context.SaveChangesAsync();
                
                var genreDto = new GenreDto
                {
                    Id = genre.Id,
                    Name = genre.Name,
                    Description = genre.Description,
                    CreatedAt = genre.CreatedAt,
                    UpdatedAt = genre.UpdatedAt,
                    BooksCount = await _context.Books.CountAsync(b => b.GenreId == genre.Id)
                };
                
                return ApiResponse<GenreDto>.SuccessResponse(genreDto, "Gênero atualizado com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<GenreDto>.ErrorResponse("Erro ao atualizar gênero");
            }
        }
        
        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var genre = await _context.Genres
                    .Include(g => g.Books)
                    .FirstOrDefaultAsync(g => g.Id == id);
                
                if (genre == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Gênero não encontrado");
                }
                
                if (genre.Books.Any())
                {
                    return ApiResponse<bool>.ErrorResponse("Não é possível excluir um gênero que possui livros associados");
                }
                
                _context.Genres.Remove(genre);
                await _context.SaveChangesAsync();
                
                return ApiResponse<bool>.SuccessResponse(true, "Gênero excluído com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse("Erro ao excluir gênero");
            }
        }
        
        public async Task<ApiResponse<bool>> ExistsAsync(int id)
        {
            try
            {
                var exists = await _context.Genres.AnyAsync(g => g.Id == id);
                return ApiResponse<bool>.SuccessResponse(exists);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse("Erro ao verificar existência do gênero");
            }
        }
    }
}

