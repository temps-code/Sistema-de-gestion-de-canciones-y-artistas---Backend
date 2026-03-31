namespace Disquera.Dtos.Artistas
{
    // Información completa de un Artista (para detalles)
    public class ArtistaDto
    {
        public int ArtistaId { get; set; }
        public string Nombre { get; set; }
        public string? Nacionalidad { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
