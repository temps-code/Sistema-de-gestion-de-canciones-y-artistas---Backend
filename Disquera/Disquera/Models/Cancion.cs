using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Disquera.Models
{
    public class Cancion
    {
        [Key]
        public int CancionId { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(255)]
        public string Titulo { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La duración debe ser mayor que 0.")]
        public int Duracion { get; set; } // en minutos

        [StringLength(100)]
        public string? Genero { get; set; }

        // Clave foránea hacia Artista
        [Required(ErrorMessage = "Es obligatorio seleccionar un artista.")]
        public int ArtistaId { get; set; }

        [ForeignKey("ArtistaId")]
        public Artista? Artista { get; set; }

        // Borrado lógico
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
