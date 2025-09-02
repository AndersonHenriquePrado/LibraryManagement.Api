using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Services;

namespace LibraryManagement.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        
        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }
        
        /// <summary>
        /// Recupera todos os livros com paginação e filtros
        /// </summary>
        /// <param name="pageNumber">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Tamanho da página (padrão: 10)</param>
        /// <param name="searchTerm">Termo de busca (opcional)</param>
        /// <param name="authorId">ID do autor para filtrar (opcional)</param>
        /// <param name="genreId">ID do gênero para filtrar (opcional)</param>
        /// <returns>Lista paginada de livros</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<BookDto>), 200)]
        public async Task<ActionResult<PagedResponse<BookDto>>> GetAll(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? authorId = null,
            [FromQuery] int? genreId = null)
        {
            var result = await _bookService.GetAllAsync(pageNumber, pageSize, searchTerm, authorId, genreId);
            return Ok(result);
        }
        
        /// <summary>
        /// Recupera um livro específico por ID
        /// </summary>
        /// <param name="id">ID do livro</param>
        /// <returns>Dados do livro</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<BookDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<BookDto>), 404)]
        public async Task<ActionResult<ApiResponse<BookDto>>> GetById(int id)
        {
            var result = await _bookService.GetByIdAsync(id);
            
            if (!result.Success)
            {
                return NotFound(result);
            }
            
            return Ok(result);
        }
        
        /// <summary>
        /// Cria um novo livro
        /// </summary>
        /// <param name="createBookDto">Dados do livro a ser criado</param>
        /// <returns>Dados do livro criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<BookDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<BookDto>), 400)]
        public async Task<ActionResult<ApiResponse<BookDto>>> Create([FromBody] CreateBookDto createBookDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                    
                return BadRequest(ApiResponse<BookDto>.ErrorResponse("Dados inválidos", errors));
            }
            
            var result = await _bookService.CreateAsync(createBookDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }
            
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
        }
        
        /// <summary>
        /// Atualiza um livro existente
        /// </summary>
        /// <param name="id">ID do livro</param>
        /// <param name="updateBookDto">Dados atualizados do livro</param>
        /// <returns>Dados do livro atualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<BookDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<BookDto>), 400)]
        [ProducesResponseType(typeof(ApiResponse<BookDto>), 404)]
        public async Task<ActionResult<ApiResponse<BookDto>>> Update(int id, [FromBody] UpdateBookDto updateBookDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                    
                return BadRequest(ApiResponse<BookDto>.ErrorResponse("Dados inválidos", errors));
            }
            
            var result = await _bookService.UpdateAsync(id, updateBookDto);
            
            if (!result.Success)
            {
                if (result.Message.Contains("não encontrado"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }
            
            return Ok(result);
        }
        
        /// <summary>
        /// Exclui um livro
        /// </summary>
        /// <param name="id">ID do livro</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 404)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var result = await _bookService.DeleteAsync(id);
            
            if (!result.Success)
            {
                if (result.Message.Contains("não encontrado"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }
            
            return Ok(result);
        }
        
        /// <summary>
        /// Verifica se um livro existe
        /// </summary>
        /// <param name="id">ID do livro</param>
        /// <returns>True se o livro existe, False caso contrário</returns>
        [HttpHead("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Exists(int id)
        {
            var result = await _bookService.ExistsAsync(id);
            
            if (result.Success && result.Data == true)
            {
                return Ok();
            }
            
            return NotFound();
        }
    }
}

