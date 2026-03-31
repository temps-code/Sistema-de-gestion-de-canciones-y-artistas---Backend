using System;
using System.ComponentModel.DataAnnotations;

namespace Disquera.Models
{
    public class Artista
    {
        [Key]
        public int ArtistaId { get; set; }

        [Required]
        [StringLength(255)]
        public string Nombre { get; set; }

        [StringLength(100)]
        public string? Nacionalidad { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        // Borrado lógico
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
