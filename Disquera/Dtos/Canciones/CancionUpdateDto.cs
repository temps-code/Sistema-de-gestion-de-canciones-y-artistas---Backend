using System.ComponentModel.DataAnnotations;

namespace Disquera.Dtos.Canciones
{
    // Para actualizar los campos de una canción existente
    public class CancionUpdateDto
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(255, ErrorMessage = "Máximo 255 caracteres.")]
        public string Titulo { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La duración debe ser mayor que 0.")]
        public int Duracion { get; set; }

        [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
        public string? Genero { get; set; }

        [Required(ErrorMessage = "Es obligatorio seleccionar un artista.")]
        public int ArtistaId { get; set; }
    }
}
