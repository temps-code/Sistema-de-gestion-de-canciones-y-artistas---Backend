using System.ComponentModel.DataAnnotations;

namespace Disquera.Dtos.Artistas
{
    // Para crear un nuevo artista
    public class ArtistaCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(255, ErrorMessage = "Máximo 255 caracteres.")]
        public string Nombre { get; set; }

        [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
        public string? Nacionalidad { get; set; }

        public DateTime? FechaNacimiento { get; set; }
    }
}
