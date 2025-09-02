using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Services;

namespace LibraryManagement.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class GenresController : ControllerBase
    {
        private readonly IGenreService _genreService;
        
        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }
        
        /// <summary>
        /// Recupera todos os gêneros com paginação
        /// </summary>
        /// <param name="pageNumber">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Tamanho da página (padrão: 10)</param>
        /// <param name="searchTerm">Termo de busca (opcional)</param>
        /// <returns>Lista paginada de gêneros</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<GenreDto>), 200)]
        public async Task<ActionResult<PagedResponse<GenreDto>>> GetAll(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] string? searchTerm = null)
        {
            var result = await _genreService.GetAllAsync(pageNumber, pageSize, searchTerm);
            return Ok(result);
        }
        
        /// <summary>
        /// Recupera um gênero específico por ID
        /// </summary>
        /// <param name="id">ID do gênero</param>
        /// <returns>Dados do gênero</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<GenreDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<GenreDto>), 404)]
        public async Task<ActionResult<ApiResponse<GenreDto>>> GetById(int id)
        {
            var result = await _genreService.GetByIdAsync(id);
            
            if (!result.Success)
            {
                return NotFound(result);
            }
            
            return Ok(result);
        }
        
        /// <summary>
        /// Cria um novo gênero
        /// </summary>
        /// <param name="createGenreDto">Dados do gênero a ser criado</param>
        /// <returns>Dados do gênero criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GenreDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<GenreDto>), 400)]
        public async Task<ActionResult<ApiResponse<GenreDto>>> Create([FromBody] CreateGenreDto createGenreDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                    
                return BadRequest(ApiResponse<GenreDto>.ErrorResponse("Dados inválidos", errors));
            }
            
            var result = await _genreService.CreateAsync(createGenreDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }
            
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
        }
        
        /// <summary>
        /// Atualiza um gênero existente
        /// </summary>
        /// <param name="id">ID do gênero</param>
        /// <param name="updateGenreDto">Dados atualizados do gênero</param>
        /// <returns>Dados do gênero atualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<GenreDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<GenreDto>), 400)]
        [ProducesResponseType(typeof(ApiResponse<GenreDto>), 404)]
        public async Task<ActionResult<ApiResponse<GenreDto>>> Update(int id, [FromBody] UpdateGenreDto updateGenreDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                    
                return BadRequest(ApiResponse<GenreDto>.ErrorResponse("Dados inválidos", errors));
            }
            
            var result = await _genreService.UpdateAsync(id, updateGenreDto);
            
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
        /// Exclui um gênero
        /// </summary>
        /// <param name="id">ID do gênero</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 404)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var result = await _genreService.DeleteAsync(id);
            
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
        /// Verifica se um gênero existe
        /// </summary>
        /// <param name="id">ID do gênero</param>
        /// <returns>True se o gênero existe, False caso contrário</returns>
        [HttpHead("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Exists(int id)
        {
            var result = await _genreService.ExistsAsync(id);
            
            if (result.Success && result.Data == true)
            {
                return Ok();
            }
            
            return NotFound();
        }
    }
}

