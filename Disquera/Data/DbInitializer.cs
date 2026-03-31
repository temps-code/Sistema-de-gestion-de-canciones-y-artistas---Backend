using System;
using System.Linq;
using Disquera.Models;

namespace Disquera.Data
{
    public static class DbInitializer
    {
        public static void Initialize(DisqueraContext context)
        {
            // Asegurarse de que la base de datos esté creada
            context.Database.EnsureCreated();

            // Si ya hay artistas registrados, no hacemos nada
            if (context.Artistas.Any())
            {
                return;   // La BD ya fue sembrada
            }

            // 1) Sembrar Artistas
            var artistas = new Artista[]
            {
                new Artista
                {
                    Nombre = "Artista A",
                    Nacionalidad = "Argentina",
                    FechaNacimiento = new DateTime(1990, 5, 1),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Artista
                {
                    Nombre = "Artista B",
                    Nacionalidad = "España",
                    FechaNacimiento = new DateTime(1985, 7, 12),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Artista
                {
                    Nombre = "Artista C",
                    Nacionalidad = "México",
                    FechaNacimiento = new DateTime(1992, 3, 20),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            foreach (var artista in artistas)
            {
                context.Artistas.Add(artista);
            }
            context.SaveChanges();

            // 2) Sembrar Canciones (referenciando a los artistas recién creados)
            if (!context.Canciones.Any())
            {
                // Obtener los IDs de los artistas recién insertados
                var artistaA = context.Artistas.Single(a => a.Nombre == "Artista A");
                var artistaB = context.Artistas.Single(a => a.Nombre == "Artista B");
                var artistaC = context.Artistas.Single(a => a.Nombre == "Artista C");

                var canciones = new Cancion[]
                {
                    new Cancion
                    {
                        Titulo = "Éxito Musical",
                        Duracion = 5,
                        Genero = "Pop",
                        ArtistaId = artistaA.ArtistaId,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Cancion
                    {
                        Titulo = "Rock en la Noche",
                        Duracion = 4,
                        Genero = "Rock",
                        ArtistaId = artistaB.ArtistaId,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Cancion
                    {
                        Titulo = "Jazz Suave",
                        Duracion = 6,
                        Genero = "Jazz",
                        ArtistaId = artistaC.ArtistaId,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                foreach (var cancion in canciones)
                {
                    context.Canciones.Add(cancion);
                }
                context.SaveChanges();
            }
        }
    }
}
