using LibraryManagement.Api.DTOs;

namespace LibraryManagement.Api.Services
{
    public interface IAuthorService
    {
        Task<PagedResponse<AuthorDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null);
        Task<ApiResponse<AuthorDto>> GetByIdAsync(int id);
        Task<ApiResponse<AuthorDto>> CreateAsync(CreateAuthorDto createAuthorDto);
        Task<ApiResponse<AuthorDto>> UpdateAsync(int id, UpdateAuthorDto updateAuthorDto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<bool>> ExistsAsync(int id);
    }
}

