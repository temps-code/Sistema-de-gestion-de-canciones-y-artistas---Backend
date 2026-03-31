using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Disquera.Data;
using Disquera.Dtos.Artistas;
using Disquera.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Disquera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArtistasController : ControllerBase
    {
        private readonly DisqueraContext _context;

        public ArtistasController(DisqueraContext context)
        {
            _context = context;
        }

        /// GET api/artistas/{id}
        /// Obtiene un artista por su ID (activo o inactivo).
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var artista = await _context.Artistas
                .FirstOrDefaultAsync(a => a.ArtistaId == id);

            if (artista == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"No se encontró ningún artista con ID = {id}."
                });
            }

            var dto = new ArtistaDto
            {
                ArtistaId = artista.ArtistaId,
                Nombre = artista.Nombre,
                Nacionalidad = artista.Nacionalidad,
                FechaNacimiento = artista.FechaNacimiento,
                IsActive = artista.IsActive,
                CreatedAt = artista.CreatedAt,
                UpdatedAt = artista.UpdatedAt
            };

            return Ok(new
            {
                success = true,
                message = $"Artista (ID = {id}) obtenido correctamente.",
                data = dto
            });
        }

        /// GET api/artistas/activos
        /// Lista sólo artistas activos.
        [HttpGet("activos")]
        public async Task<IActionResult> GetActivos()
        {
            var lista = await _context.Artistas
                .Where(a => a.IsActive)
                .Select(a => new ArtistaListDto
                {
                    ArtistaId = a.ArtistaId,
                    Nombre = a.Nombre,
                    IsActive = a.IsActive
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                message = lista.Any()
                    ? "Artistas activos obtenidos correctamente."
                    : "No hay artistas activos en el sistema.",
                data = lista
            });
        }

        /// GET api/artistas/inactivos
        /// Lista sólo artistas inactivos.
        [HttpGet("inactivos")]
        public async Task<IActionResult> GetInactivos()
        {
            var lista = await _context.Artistas
                .Where(a => !a.IsActive)
                .Select(a => new ArtistaListDto
                {
                    ArtistaId = a.ArtistaId,
                    Nombre = a.Nombre,
                    IsActive = a.IsActive
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                message = lista.Any()
                    ? "Artistas inactivos obtenidos correctamente."
                    : "No hay artistas inactivos en el sistema.",
                data = lista
            });
        }

        /// GET api/artistas/todos
        /// Lista todos los artistas (activos e inactivos).
        [HttpGet("todos")]
        public async Task<IActionResult> GetTodos()
        {
            var lista = await _context.Artistas
                .Select(a => new ArtistaListDto
                {
                    ArtistaId = a.ArtistaId,
                    Nombre = a.Nombre,
                    IsActive = a.IsActive
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                message = lista.Any()
                    ? "Todos los artistas obtenidos correctamente."
                    : "No hay artistas registrados en el sistema.",
                data = lista
            });
        }

        /// POST api/artistas
        /// Crea un nuevo artista.
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ArtistaCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                // Extraigo errores de ModelState para devolverlos en "errors"
                var errores = ModelState
                    .Where(kvp => kvp.Value.Errors.Any())
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new
                {
                    success = false,
                    message = "Datos inválidos para crear artista.",
                    errors = errores
                });
            }

            var artista = new Artista
            {
                Nombre = dto.Nombre.Trim(),
                Nacionalidad = dto.Nacionalidad?.Trim(),
                FechaNacimiento = dto.FechaNacimiento,
                IsActive = true,
                CreatedAt = System.DateTime.UtcNow,
                UpdatedAt = System.DateTime.UtcNow
            };

            _context.Artistas.Add(artista);
            await _context.SaveChangesAsync();

            var resultDto = new ArtistaDto
            {
                ArtistaId = artista.ArtistaId,
                Nombre = artista.Nombre,
                Nacionalidad = artista.Nacionalidad,
                FechaNacimiento = artista.FechaNacimiento,
                IsActive = artista.IsActive,
                CreatedAt = artista.CreatedAt,
                UpdatedAt = artista.UpdatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = artista.ArtistaId }, new
            {
                success = true,
                message = "Artista creado exitosamente.",
                data = resultDto
            });
        }

        /// PUT api/artistas/{id}
        /// Actualiza un artista existente (sólo si está activo).
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ArtistaUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState
                    .Where(kvp => kvp.Value.Errors.Any())
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new
                {
                    success = false,
                    message = "Datos inválidos para actualizar artista.",
                    errors = errores
                });
            }

            var artista = await _context.Artistas
                .FirstOrDefaultAsync(a => a.ArtistaId == id && a.IsActive);

            if (artista == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"No se encontró un artista activo con ID = {id}."
                });
            }

            artista.Nombre = dto.Nombre.Trim();
            artista.Nacionalidad = dto.Nacionalidad?.Trim();
            artista.FechaNacimiento = dto.FechaNacimiento;
            artista.UpdatedAt = System.DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = $"Artista (ID = {id}) actualizado correctamente."
            });
        }

        /// DELETE api/artistas/{id}
        /// Borrado lógico: marca IsActive = false (si estaba activo).
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteLogico(int id)
        {
            var artista = await _context.Artistas
                .FirstOrDefaultAsync(a => a.ArtistaId == id && a.IsActive);

            if (artista == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"No se encontró un artista activo con ID = {id}."
                });
            }

            artista.IsActive = false;
            artista.UpdatedAt = System.DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = $"Artista (ID = {id}) eliminado lógicamente correctamente."
            });
        }

        /// DELETE api/artistas/{id}/fisico
        /// Borrado físico: elimina el registro de la tabla (activo o inactivo).
        [HttpDelete("{id:int}/fisico")]
        public async Task<IActionResult> DeleteFisico(int id)
        {
            var artista = await _context.Artistas
                .FirstOrDefaultAsync(a => a.ArtistaId == id);

            if (artista == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"No se encontró ningún artista con ID = {id}."
                });
            }

            _context.Artistas.Remove(artista);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = $"Artista (ID = {id}) eliminado físicamente correctamente."
            });
        }

        /// PUT api/artistas/{id}/reactivar
        /// Reactiva un artista que se había borrado lógicamente (IsActive = false).
        [HttpPut("{id:int}/reactivar")]
        public async Task<IActionResult> Reactivar(int id)
        {
            var artista = await _context.Artistas
                .FirstOrDefaultAsync(a => a.ArtistaId == id && !a.IsActive);

            if (artista == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"No se encontró un artista inactivo con ID = {id}."
                });
            }

            artista.IsActive = true;
            artista.UpdatedAt = System.DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = $"Artista (ID = {id}) reactivado correctamente."
            });
        }
    }
}
