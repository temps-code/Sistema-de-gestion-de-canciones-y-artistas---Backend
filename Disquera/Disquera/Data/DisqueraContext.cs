using Disquera.Models;
using Microsoft.EntityFrameworkCore;

namespace Disquera.Data
{
    public class DisqueraContext : DbContext
    {
        public DisqueraContext(DbContextOptions<DisqueraContext> options)
            : base(options)
        {
        }

        public DbSet<Artista> Artistas { get; set; }
        public DbSet<Cancion> Canciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cancion>()
                .HasCheckConstraint("CK_Cancion_Duracion", "[Duracion] > 0");

            modelBuilder.Entity<Cancion>()
                .HasOne(c => c.Artista)
                .WithMany()
                .HasForeignKey(c => c.ArtistaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
