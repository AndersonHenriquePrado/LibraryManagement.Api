using Microsoft.EntityFrameworkCore;
using LibraryManagement.Api.Data;
using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Models;

namespace LibraryManagement.Api.Services
{
    public class BookService : IBookService
    {
        private readonly LibraryContext _context;
        
        public BookService(LibraryContext context)
        {
            _context = context;
        }
        
        public async Task<PagedResponse<BookDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, int? authorId = null, int? genreId = null)
        {
            try
            {
                var query = _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Genre)
                    .AsQueryable();
                
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(b => b.Title.Contains(searchTerm) || 
                                           (b.Description != null && b.Description.Contains(searchTerm)) ||
                                           (b.ISBN != null && b.ISBN.Contains(searchTerm)) ||
                                           (b.Publisher != null && b.Publisher.Contains(searchTerm)));
                }
                
                if (authorId.HasValue)
                {
                    query = query.Where(b => b.AuthorId == authorId.Value);
                }
                
                if (genreId.HasValue)
                {
                    query = query.Where(b => b.GenreId == genreId.Value);
                }
                
                var totalCount = await query.CountAsync();
                
                var books = await query
                    .OrderBy(b => b.Title)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(b => new BookDto
                    {
                        Id = b.Id,
                        Title = b.Title,
                        ISBN = b.ISBN,
                        Description = b.Description,
                        PublicationDate = b.PublicationDate,
                        Publisher = b.Publisher,
                        Pages = b.Pages,
                        Price = b.Price,
                        CreatedAt = b.CreatedAt,
                        UpdatedAt = b.UpdatedAt,
                        AuthorId = b.AuthorId,
                        AuthorName = b.Author.FirstName + " " + b.Author.LastName,
                        GenreId = b.GenreId,
                        GenreName = b.Genre.Name
                    })
                    .ToListAsync();
                
                return PagedResponse<BookDto>.Create(books, totalCount, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                return new PagedResponse<BookDto>
                {
                    Success = false,
                    Message = "Erro ao recuperar livros",
                    Data = new List<BookDto>()
                };
            }
        }
        
        public async Task<ApiResponse<BookDto>> GetByIdAsync(int id)
        {
            try
            {
                var book = await _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Genre)
                    .FirstOrDefaultAsync(b => b.Id == id);
                
                if (book == null)
                {
                    return ApiResponse<BookDto>.ErrorResponse("Livro não encontrado");
                }
                
                var bookDto = new BookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    ISBN = book.ISBN,
                    Description = book.Description,
                    PublicationDate = book.PublicationDate,
                    Publisher = book.Publisher,
                    Pages = book.Pages,
                    Price = book.Price,
                    CreatedAt = book.CreatedAt,
                    UpdatedAt = book.UpdatedAt,
                    AuthorId = book.AuthorId,
                    AuthorName = book.Author.FullName,
                    GenreId = book.GenreId,
                    GenreName = book.Genre.Name
                };
                
                return ApiResponse<BookDto>.SuccessResponse(bookDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<BookDto>.ErrorResponse("Erro ao recuperar livro");
            }
        }
        
        public async Task<ApiResponse<BookDto>> CreateAsync(CreateBookDto createBookDto)
        {
            try
            {
                // Validate author exists
                var authorExists = await _context.Authors.AnyAsync(a => a.Id == createBookDto.AuthorId);
                if (!authorExists)
                {
                    return ApiResponse<BookDto>.ErrorResponse("Autor não encontrado");
                }
                
                // Validate genre exists
                var genreExists = await _context.Genres.AnyAsync(g => g.Id == createBookDto.GenreId);
                if (!genreExists)
                {
                    return ApiResponse<BookDto>.ErrorResponse("Gênero não encontrado");
                }
                
                // Check if book with same ISBN already exists
                if (!string.IsNullOrWhiteSpace(createBookDto.ISBN))
                {
                    var existingBook = await _context.Books
                        .FirstOrDefaultAsync(b => b.ISBN == createBookDto.ISBN);
                    
                    if (existingBook != null)
                    {
                        return ApiResponse<BookDto>.ErrorResponse("Já existe um livro com este ISBN");
                    }
                }
                
                var book = new Book
                {
                    Title = createBookDto.Title,
                    ISBN = createBookDto.ISBN,
                    Description = createBookDto.Description,
                    PublicationDate = createBookDto.PublicationDate,
                    Publisher = createBookDto.Publisher,
                    Pages = createBookDto.Pages,
                    Price = createBookDto.Price,
                    AuthorId = createBookDto.AuthorId,
                    GenreId = createBookDto.GenreId
                };
                
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                
                // Load related entities for response
                await _context.Entry(book)
                    .Reference(b => b.Author)
                    .LoadAsync();
                    
                await _context.Entry(book)
                    .Reference(b => b.Genre)
                    .LoadAsync();
                
                var bookDto = new BookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    ISBN = book.ISBN,
                    Description = book.Description,
                    PublicationDate = book.PublicationDate,
                    Publisher = book.Publisher,
                    Pages = book.Pages,
                    Price = book.Price,
                    CreatedAt = book.CreatedAt,
                    UpdatedAt = book.UpdatedAt,
                    AuthorId = book.AuthorId,
                    AuthorName = book.Author.FullName,
                    GenreId = book.GenreId,
                    GenreName = book.Genre.Name
                };
                
                return ApiResponse<BookDto>.SuccessResponse(bookDto, "Livro criado com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<BookDto>.ErrorResponse("Erro ao criar livro");
            }
        }
        
        public async Task<ApiResponse<BookDto>> UpdateAsync(int id, UpdateBookDto updateBookDto)
        {
            try
            {
                var book = await _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Genre)
                    .FirstOrDefaultAsync(b => b.Id == id);
                
                if (book == null)
                {
                    return ApiResponse<BookDto>.ErrorResponse("Livro não encontrado");
                }
                
                // Validate author exists
                var authorExists = await _context.Authors.AnyAsync(a => a.Id == updateBookDto.AuthorId);
                if (!authorExists)
                {
                    return ApiResponse<BookDto>.ErrorResponse("Autor não encontrado");
                }
                
                // Validate genre exists
                var genreExists = await _context.Genres.AnyAsync(g => g.Id == updateBookDto.GenreId);
                if (!genreExists)
                {
                    return ApiResponse<BookDto>.ErrorResponse("Gênero não encontrado");
                }
                
                // Check if another book with same ISBN already exists
                if (!string.IsNullOrWhiteSpace(updateBookDto.ISBN))
                {
                    var existingBook = await _context.Books
                        .FirstOrDefaultAsync(b => b.ISBN == updateBookDto.ISBN && b.Id != id);
                    
                    if (existingBook != null)
                    {
                        return ApiResponse<BookDto>.ErrorResponse("Já existe outro livro com este ISBN");
                    }
                }
                
                book.Title = updateBookDto.Title;
                book.ISBN = updateBookDto.ISBN;
                book.Description = updateBookDto.Description;
                book.PublicationDate = updateBookDto.PublicationDate;
                book.Publisher = updateBookDto.Publisher;
                book.Pages = updateBookDto.Pages;
                book.Price = updateBookDto.Price;
                book.AuthorId = updateBookDto.AuthorId;
                book.GenreId = updateBookDto.GenreId;
                
                await _context.SaveChangesAsync();
                
                // Reload related entities if they changed
                if (book.AuthorId != book.Author?.Id)
                {
                    await _context.Entry(book)
                        .Reference(b => b.Author)
                        .LoadAsync();
                }
                
                if (book.GenreId != book.Genre?.Id)
                {
                    await _context.Entry(book)
                        .Reference(b => b.Genre)
                        .LoadAsync();
                }
                
                var bookDto = new BookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    ISBN = book.ISBN,
                    Description = book.Description,
                    PublicationDate = book.PublicationDate,
                    Publisher = book.Publisher,
                    Pages = book.Pages,
                    Price = book.Price,
                    CreatedAt = book.CreatedAt,
                    UpdatedAt = book.UpdatedAt,
                    AuthorId = book.AuthorId,
                    AuthorName = book.Author.FullName,
                    GenreId = book.GenreId,
                    GenreName = book.Genre.Name
                };
                
                return ApiResponse<BookDto>.SuccessResponse(bookDto, "Livro atualizado com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<BookDto>.ErrorResponse("Erro ao atualizar livro");
            }
        }
        
        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                
                if (book == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Livro não encontrado");
                }
                
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                
                return ApiResponse<bool>.SuccessResponse(true, "Livro excluído com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse("Erro ao excluir livro");
            }
        }
        
        public async Task<ApiResponse<bool>> ExistsAsync(int id)
        {
            try
            {
                var exists = await _context.Books.AnyAsync(b => b.Id == id);
                return ApiResponse<bool>.SuccessResponse(exists);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse("Erro ao verificar existência do livro");
            }
        }
    }
}

