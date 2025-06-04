using System.ComponentModel.DataAnnotations;

namespace Disquera.Dtos.Artistas
{
    // Para actualizar un artista existente
    public class ArtistaUpdateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(255, ErrorMessage = "Máximo 255 caracteres.")]
        public string Nombre { get; set; }

        [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
        public string? Nacionalidad { get; set; }

        public DateTime? FechaNacimiento { get; set; }
    }
}
