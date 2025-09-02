using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Services;

namespace LibraryManagement.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        
        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }
        
        /// <summary>
        /// Recupera todos os autores com paginação
        /// </summary>
        /// <param name="pageNumber">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Tamanho da página (padrão: 10)</param>
        /// <param name="searchTerm">Termo de busca (opcional)</param>
        /// <returns>Lista paginada de autores</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<AuthorDto>), 200)]
        public async Task<ActionResult<PagedResponse<AuthorDto>>> GetAll(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] string? searchTerm = null)
        {
            var result = await _authorService.GetAllAsync(pageNumber, pageSize, searchTerm);
            return Ok(result);
        }
        
        /// <summary>
        /// Recupera um autor específico por ID
        /// </summary>
        /// <param name="id">ID do autor</param>
        /// <returns>Dados do autor</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AuthorDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<AuthorDto>), 404)]
        public async Task<ActionResult<ApiResponse<AuthorDto>>> GetById(int id)
        {
            var result = await _authorService.GetByIdAsync(id);
            
            if (!result.Success)
            {
                return NotFound(result);
            }
            
            return Ok(result);
        }
        
        /// <summary>
        /// Cria um novo autor
        /// </summary>
        /// <param name="createAuthorDto">Dados do autor a ser criado</param>
        /// <returns>Dados do autor criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AuthorDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<AuthorDto>), 400)]
        public async Task<ActionResult<ApiResponse<AuthorDto>>> Create([FromBody] CreateAuthorDto createAuthorDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                    
                return BadRequest(ApiResponse<AuthorDto>.ErrorResponse("Dados inválidos", errors));
            }
            
            var result = await _authorService.CreateAsync(createAuthorDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }
            
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
        }
        
        /// <summary>
        /// Atualiza um autor existente
        /// </summary>
        /// <param name="id">ID do autor</param>
        /// <param name="updateAuthorDto">Dados atualizados do autor</param>
        /// <returns>Dados do autor atualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AuthorDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<AuthorDto>), 400)]
        [ProducesResponseType(typeof(ApiResponse<AuthorDto>), 404)]
        public async Task<ActionResult<ApiResponse<AuthorDto>>> Update(int id, [FromBody] UpdateAuthorDto updateAuthorDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                    
                return BadRequest(ApiResponse<AuthorDto>.ErrorResponse("Dados inválidos", errors));
            }
            
            var result = await _authorService.UpdateAsync(id, updateAuthorDto);
            
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
        /// Exclui um autor
        /// </summary>
        /// <param name="id">ID do autor</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 404)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var result = await _authorService.DeleteAsync(id);
            
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
        /// Verifica se um autor existe
        /// </summary>
        /// <param name="id">ID do autor</param>
        /// <returns>True se o autor existe, False caso contrário</returns>
        [HttpHead("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Exists(int id)
        {
            var result = await _authorService.ExistsAsync(id);
            
            if (result.Success && result.Data == true)
            {
                return Ok();
            }
            
            return NotFound();
        }
    }
}

