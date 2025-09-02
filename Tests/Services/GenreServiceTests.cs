using Microsoft.EntityFrameworkCore;
using Xunit;
using LibraryManagement.Api.Data;
using LibraryManagement.Api.Services;
using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Models;

namespace LibraryManagement.Api.Tests.Services
{
    public class GenreServiceTests : IDisposable
    {
        private readonly LibraryContext _context;
        private readonly GenreService _genreService;

        public GenreServiceTests()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new LibraryContext(options);
            _genreService = new GenreService(_context);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsPagedResponse()
        {
            // Arrange
            var genre1 = new Genre { Id = 1, Name = "Ficção", Description = "Livros de ficção" };
            var genre2 = new Genre { Id = 2, Name = "Romance", Description = "Livros românticos" };
            
            _context.Genres.AddRange(genre1, genre2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _genreService.GetAllAsync(1, 10);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Data.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsGenre()
        {
            // Arrange
            var genre = new Genre { Id = 1, Name = "Ficção", Description = "Livros de ficção" };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            // Act
            var result = await _genreService.GetByIdAsync(1);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Ficção", result.Data.Name);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsError()
        {
            // Act
            var result = await _genreService.GetByIdAsync(999);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("não encontrado", result.Message);
        }

        [Fact]
        public async Task CreateAsync_ValidGenre_ReturnsSuccess()
        {
            // Arrange
            var createDto = new CreateGenreDto
            {
                Name = "Ficção Científica",
                Description = "Livros de ficção científica"
            };

            // Act
            var result = await _genreService.CreateAsync(createDto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Ficção Científica", result.Data.Name);
        }

        [Fact]
        public async Task CreateAsync_DuplicateName_ReturnsError()
        {
            // Arrange
            var existingGenre = new Genre { Id = 1, Name = "Ficção" };
            _context.Genres.Add(existingGenre);
            await _context.SaveChangesAsync();

            var createDto = new CreateGenreDto { Name = "Ficção" };

            // Act
            var result = await _genreService.CreateAsync(createDto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("já existe", result.Message.ToLower());
        }

        [Fact]
        public async Task UpdateAsync_ValidGenre_ReturnsSuccess()
        {
            // Arrange
            var genre = new Genre { Id = 1, Name = "Ficção", Description = "Descrição antiga" };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateGenreDto
            {
                Name = "Ficção Atualizada",
                Description = "Nova descrição"
            };

            // Act
            var result = await _genreService.UpdateAsync(1, updateDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Ficção Atualizada", result.Data.Name);
            Assert.Equal("Nova descrição", result.Data.Description);
        }

        [Fact]
        public async Task DeleteAsync_GenreWithoutBooks_ReturnsSuccess()
        {
            // Arrange
            var genre = new Genre { Id = 1, Name = "Ficção" };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            // Act
            var result = await _genreService.DeleteAsync(1);

            // Assert
            Assert.True(result.Success);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task DeleteAsync_GenreWithBooks_ReturnsError()
        {
            // Arrange
            var author = new Author { Id = 1, FirstName = "Test", LastName = "Author" };
            var genre = new Genre { Id = 1, Name = "Ficção" };
            var book = new Book { Id = 1, Title = "Test Book", AuthorId = 1, GenreId = 1 };
            
            _context.Authors.Add(author);
            _context.Genres.Add(genre);
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Act
            var result = await _genreService.DeleteAsync(1);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("livros associados", result.Message.ToLower());
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

