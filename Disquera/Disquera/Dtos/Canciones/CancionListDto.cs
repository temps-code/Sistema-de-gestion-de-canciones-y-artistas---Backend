namespace Disquera.Dtos.Canciones
{
    // Para listar canciones resumidas
    public class CancionListDto
    {
        public int CancionId { get; set; }
        public string Titulo { get; set; }
        public int Duracion { get; set; }
        public string? Genero { get; set; }
        public int ArtistaId { get; set; }
        public string ArtistaNombre { get; set; }
        public bool IsActive { get; set; }
    }
}
