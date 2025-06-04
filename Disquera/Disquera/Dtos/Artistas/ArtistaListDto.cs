namespace Disquera.Dtos.Artistas
{
    // Para listar artistas (resumen). Incluye sólo campos clave y nombre.
    public class ArtistaListDto
    {
        public int ArtistaId { get; set; }
        public string Nombre { get; set; }
        public bool IsActive { get; set; }
    }
}
