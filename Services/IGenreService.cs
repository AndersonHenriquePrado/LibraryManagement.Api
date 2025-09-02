using LibraryManagement.Api.DTOs;

namespace LibraryManagement.Api.Services
{
    public interface IGenreService
    {
        Task<PagedResponse<GenreDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null);
        Task<ApiResponse<GenreDto>> GetByIdAsync(int id);
        Task<ApiResponse<GenreDto>> CreateAsync(CreateGenreDto createGenreDto);
        Task<ApiResponse<GenreDto>> UpdateAsync(int id, UpdateGenreDto updateGenreDto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<bool>> ExistsAsync(int id);
    }
}

