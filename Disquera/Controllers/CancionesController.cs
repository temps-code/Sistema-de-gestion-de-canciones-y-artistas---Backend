using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Disquera.Data;
using Disquera.Dtos.Canciones;
using Disquera.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Disquera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CancionesController : ControllerBase
    {
        private readonly DisqueraContext _context;

        public CancionesController(DisqueraContext context)
        {
            _context = context;
        }

        /// GET api/canciones/{id}
        /// Obtiene una canción por su ID (activo o inactivo).
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cancion = await _context.Canciones
                .Include(c => c.Artista)
                .FirstOrDefaultAsync(c => c.CancionId == id);

            if (cancion == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"No se encontró ninguna canción con ID = {id}."
                });
            }

            var dto = new CancionDto
            {
                CancionId = cancion.CancionId,
                Titulo = cancion.Titulo,
                Duracion = cancion.Duracion,
                Genero = cancion.Genero,
                ArtistaId = cancion.ArtistaId,
                ArtistaNombre = cancion.Artista?.Nombre ?? string.Empty,
                IsActive = cancion.IsActive,
                CreatedAt = cancion.CreatedAt,
                UpdatedAt = cancion.UpdatedAt
            };

            return Ok(new
            {
                success = true,
                message = $"Canción (ID = {id}) obtenida correctamente.",
                data = dto
            });
        }

        /// GET api/canciones/activos
        /// Lista sólo canciones activas.
        [HttpGet("activos")]
        public async Task<IActionResult> GetActivos()
        {
            var lista = await _context.Canciones
                .Where(c => c.IsActive)
                .Include(c => c.Artista)
                .Select(c => new CancionListDto
                {
                    CancionId = c.CancionId,
                    Titulo = c.Titulo,
                    Duracion = c.Duracion,
                    Genero = c.Genero,
                    ArtistaId = c.ArtistaId,
                    ArtistaNombre = c.Artista != null ? c.Artista.Nombre : string.Empty,
                    IsActive = c.IsActive
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                message = lista.Any()
                    ? "Canciones activas obtenidas correctamente."
                    : "No hay canciones activas en el sistema.",
                data = lista
            });
        }

        /// GET api/canciones/inactivos
        /// Lista sólo canciones inactivas.
        [HttpGet("inactivos")]
        public async Task<IActionResult> GetInactivos()
        {
            var lista = await _context.Canciones
                .Where(c => !c.IsActive)
                .Include(c => c.Artista)
                .Select(c => new CancionListDto
                {
                    CancionId = c.CancionId,
                    Titulo = c.Titulo,
                    Duracion = c.Duracion,
                    Genero = c.Genero,
                    ArtistaId = c.ArtistaId,
                    ArtistaNombre = c.Artista != null ? c.Artista.Nombre : string.Empty,
                    IsActive = c.IsActive
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                message = lista.Any()
                    ? "Canciones inactivas obtenidas correctamente."
                    : "No hay canciones inactivas en el sistema.",
                data = lista
            });
        }

        /// GET api/canciones/todos
        /// Lista todas las canciones (activas e inactivas).
        [HttpGet("todos")]
        public async Task<IActionResult> GetTodos()
        {
            var lista = await _context.Canciones
                .Include(c => c.Artista)
                .Select(c => new CancionListDto
                {
                    CancionId = c.CancionId,
                    Titulo = c.Titulo,
                    Duracion = c.Duracion,
                    Genero = c.Genero,
                    ArtistaId = c.ArtistaId,
                    ArtistaNombre = c.Artista != null ? c.Artista.Nombre : string.Empty,
                    IsActive = c.IsActive
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                message = lista.Any()
                    ? "Todas las canciones obtenidas correctamente."
                    : "No hay canciones registradas en el sistema.",
                data = lista
            });
        }

        /// POST api/canciones
        /// Crea una nueva canción.
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CancionCreateDto dto)
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
                    message = "Datos inválidos para crear canción.",
                    errors = errores
                });
            }

            // Verificar que el artista exista y esté activo
            var artista = await _context.Artistas
                .FirstOrDefaultAsync(a => a.ArtistaId == dto.ArtistaId && a.IsActive);

            if (artista == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "El artista seleccionado no existe o no está activo."
                });
            }

            var cancion = new Cancion
            {
                Titulo = dto.Titulo.Trim(),
                Duracion = dto.Duracion,
                Genero = dto.Genero?.Trim(),
                ArtistaId = dto.ArtistaId,
                IsActive = true,
                CreatedAt = System.DateTime.UtcNow,
                UpdatedAt = System.DateTime.UtcNow
            };

            // Validar DataAnnotations
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(cancion);
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
                cancion, validationContext, validationResults, true);

            if (!isValid)
            {
                var errores = validationResults
                    .GroupBy(r => r.MemberNames.FirstOrDefault() ?? "")
                    .ToDictionary(
                        grp => grp.Key,
                        grp => grp.Select(r2 => r2.ErrorMessage).ToArray()
                    );

                return BadRequest(new
                {
                    success = false,
                    message = "La creación de la canción falló por errores de validación.",
                    errors = errores
                });
            }

            _context.Canciones.Add(cancion);
            await _context.SaveChangesAsync();

            var resultDto = new CancionDto
            {
                CancionId = cancion.CancionId,
                Titulo = cancion.Titulo,
                Duracion = cancion.Duracion,
                Genero = cancion.Genero,
                ArtistaId = cancion.ArtistaId,
                ArtistaNombre = artista.Nombre,
                IsActive = cancion.IsActive,
                CreatedAt = cancion.CreatedAt,
                UpdatedAt = cancion.UpdatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = cancion.CancionId }, new
            {
                success = true,
                message = "Canción creada exitosamente.",
                data = resultDto
            });
        }

        /// PUT api/canciones/{id}
        /// Actualiza una canción existente (sólo si está activa).
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CancionUpdateDto dto)
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
                    message = "Datos inválidos para actualizar canción.",
                    errors = errores
                });
            }

            var cancion = await _context.Canciones
                .FirstOrDefaultAsync(c => c.CancionId == id && c.IsActive);

            if (cancion == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"No se encontró ninguna canción activa con ID = {id}."
                });
            }

            // Verificar que el nuevo artista exista y esté activo
            var artista = await _context.Artistas
                .FirstOrDefaultAsync(a => a.ArtistaId == dto.ArtistaId && a.IsActive);

            if (artista == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "El artista seleccionado no existe o no está activo."
                });
            }

            cancion.Titulo = dto.Titulo.Trim();
            cancion.Duracion = dto.Duracion;
            cancion.Genero = dto.Genero?.Trim();
            cancion.ArtistaId = dto.ArtistaId;
            cancion.UpdatedAt = System.DateTime.UtcNow;

            // Validar nuevamente el objeto completo
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(cancion);
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
                cancion, validationContext, validationResults, true);

            if (!isValid)
            {
                var errores = validationResults
                    .GroupBy(r => r.MemberNames.FirstOrDefault() ?? "")
                    .ToDictionary(
                        grp => grp.Key,
                        grp => grp.Select(r2 => r2.ErrorMessage).ToArray()
                    );

                return BadRequest(new
                {
                    success = false,
                    message = "La actualización de la canción falló por errores de validación.",
                    errors = errores
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = $"Canción (ID = {id}) actualizada correctamente."
            });
        }

        /// DELETE api/canciones/{id}
        /// Borrado lógico: marca IsActive = false (si estaba activo).
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteLogico(int id)
        {
            var cancion = await _context.Canciones
                .FirstOrDefaultAsync(c => c.CancionId == id && c.IsActive);

            if (cancion == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"No se encontró ninguna canción activa con ID = {id}."
                });
            }

            cancion.IsActive = false;
            cancion.UpdatedAt = System.DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = $"Canción (ID = {id}) eliminada lógicamente correctamente."
            });
        }

        /// DELETE api/canciones/{id}/fisico
        /// Borrado físico: elimina el registro de la tabla (activo o inactivo).
        [HttpDelete("{id:int}/fisico")]
        public async Task<IActionResult> DeleteFisico(int id)
        {
            var cancion = await _context.Canciones
                .FirstOrDefaultAsync(c => c.CancionId == id);

            if (cancion == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"No se encontró ninguna canción con ID = {id}."
                });
            }

            _context.Canciones.Remove(cancion);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = $"Canción (ID = {id}) eliminada físicamente correctamente."
            });
        }

        /// PUT api/canciones/{id}/reactivar
        /// Reactiva una canción que se había borrado lógicamente (IsActive = false).
        [HttpPut("{id:int}/reactivar")]
        public async Task<IActionResult> Reactivar(int id)
        {
            var cancion = await _context.Canciones
                .FirstOrDefaultAsync(c => c.CancionId == id && !c.IsActive);

            if (cancion == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"No se encontró ninguna canción inactiva con ID = {id}."
                });
            }

            // Verificar que el artista asociado siga activo
            var artista = await _context.Artistas
                .FirstOrDefaultAsync(a => a.ArtistaId == cancion.ArtistaId);

            if (artista == null || !artista.IsActive)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "No se puede reactivar la canción porque su artista asociado no existe o está inactivo."
                });
            }

            cancion.IsActive = true;
            cancion.UpdatedAt = System.DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = $"Canción (ID = {id}) reactivada correctamente."
            });
        }
    }
}
