namespace Disquera.Dtos.Canciones
{
    // Detalle completo de una canción, incluyendo nombre de artista
    public class CancionDto
    {
        public int CancionId { get; set; }
        public string Titulo { get; set; }
        public int Duracion { get; set; }
        public string? Genero { get; set; }
        public int ArtistaId { get; set; }
        public string ArtistaNombre { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
