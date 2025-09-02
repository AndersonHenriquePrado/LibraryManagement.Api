using LibraryManagement.Api.DTOs;

namespace LibraryManagement.Api.Services
{
    public interface IBookService
    {
        Task<PagedResponse<BookDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, int? authorId = null, int? genreId = null);
        Task<ApiResponse<BookDto>> GetByIdAsync(int id);
        Task<ApiResponse<BookDto>> CreateAsync(CreateBookDto createBookDto);
        Task<ApiResponse<BookDto>> UpdateAsync(int id, UpdateBookDto updateBookDto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<bool>> ExistsAsync(int id);
    }
}

