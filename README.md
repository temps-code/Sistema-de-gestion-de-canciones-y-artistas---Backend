<div align="center">

<h1>Music and Artist Management System</h1>

<p><strong>REST API for managing a music catalog with artists and songs, built with ASP.NET Core 8 and Entity Framework Core.</strong></p>

<p>
  <img src="https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8">
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" alt="C#">
  <img src="https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white" alt="SQL Server">
  <img src="https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black" alt="Swagger">
</p>

</div>

---

Read this in: **English** | [Español](README.es.md)

**Academic Project**
Universidad Privada Domingo Savio — Ing. de Sistemas
Course: Information Systems II — June 2025

---

## Table of Contents

- [What It Does](#what-it-does)
- [Stack](#stack)
- [Architecture](#architecture)
- [Installation](#installation)
- [Environment Variables](#environment-variables)
- [Endpoints](#endpoints)
- [Seed Data](#seed-data)
- [Author](#author)

---

## What It Does

A backend REST API that manages a music catalog for a record label. It handles artists and songs with full CRUD operations, soft delete support, and referential integrity between entities.

- Create, read, update, and delete artists and songs
- Soft delete: records are marked inactive instead of permanently removed
- Physical delete: complete removal when required
- Reactivation of logically deleted records
- Songs can only be assigned to active artists
- Separate listing endpoints for active, inactive, and all records
- UTC timestamps tracked automatically on every record
- Structured JSON responses with validation error details
- Swagger UI available at runtime for interactive API exploration

> CORS is configured to allow any origin, enabling direct consumption from a separate frontend.

---

## Stack

| Category | Technology | Version |
|---|---|---|
| Language | C# | 12 |
| Framework | ASP.NET Core | 8.0 |
| ORM | Entity Framework Core | 8.0.16 |
| Database | Microsoft SQL Server Express | — |
| API Docs | Swashbuckle (Swagger) | 6.6.2 |

---

## Architecture

```
Disquera/
├── Controllers/
│   ├── ArtistasController.cs       # Artists REST endpoints
│   └── CancionesController.cs      # Songs REST endpoints
├── Models/
│   ├── Artista.cs                  # Artist entity
│   └── Cancion.cs                  # Song entity
├── Dtos/
│   ├── Artistas/                   # Artist DTOs (create, update, list, detail)
│   └── Canciones/                  # Song DTOs (create, update, list, detail)
├── Data/
│   ├── DisqueraContext.cs          # EF Core DbContext
│   └── DbInitializer.cs            # Database seeding on startup
├── Migrations/                     # EF Core migration history
├── appsettings.json                # App configuration and connection string
└── Program.cs                      # Service registration and middleware pipeline
```

**Data model**

```
Artista (1) ──── (N) Cancion
```

| Entity | Fields |
|---|---|
| Artista | id, nombre, nacionalidad, fechaNacimiento, isActive, createdAt, updatedAt |
| Cancion | id, titulo, duracion (min), genero, artistaId (FK), isActive, createdAt, updatedAt |

> Delete behavior is `RESTRICT`: an artist cannot be deleted while songs reference it.

---

## Installation

**Prerequisites**

- .NET 8 SDK
- SQL Server Express running locally at `localhost\SQLEXPRESS`

**Steps**

1. Clone the repository:
   ```bash
   git clone https://github.com/temps-code/Sistema-de-gestion-de-canciones-y-artistas---Backend.git
   cd Sistema-de-gestion-de-canciones-y-artistas---Backend
   ```

2. Update the connection string in `Disquera/appsettings.json` to match your local SQL Server instance.

3. Apply the database migration:
   ```bash
   cd Disquera
   dotnet ef database update
   ```

4. Run the API:
   ```bash
   dotnet run
   ```

5. Open Swagger UI in your browser:
   ```
   http://localhost:5091/swagger
   ```

> The application seeds sample data on first startup if the database is empty.

---

## Environment Variables

The connection string lives in `Disquera/appsettings.json`. Update it to match your local SQL Server configuration:

```json
{
  "ConnectionStrings": {
    "DisqueraDb": "Server=localhost\\SQLEXPRESS;Database=DisqueraDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

| Parameter | Description |
|---|---|
| `Server` | SQL Server instance name |
| `Database` | Database name (created automatically on first run) |
| `Trusted_Connection` | Use Windows Authentication |
| `TrustServerCertificate` | Required for local self-signed certificates |

---

## Endpoints

All responses follow this structure:

```json
{
  "success": true,
  "message": "...",
  "data": {},
  "errors": {}
}
```

### Artists — `/api/artistas`

| Method | Route | Description |
|---|---|---|
| GET | `/api/artistas/{id}` | Get artist by ID |
| GET | `/api/artistas/activos` | List all active artists |
| GET | `/api/artistas/inactivos` | List all inactive artists |
| GET | `/api/artistas/todos` | List all artists |
| POST | `/api/artistas` | Create artist |
| PUT | `/api/artistas/{id}` | Update artist |
| DELETE | `/api/artistas/{id}` | Soft delete artist |
| DELETE | `/api/artistas/{id}/fisico` | Physical delete artist |
| PUT | `/api/artistas/{id}/reactivar` | Reactivate artist |

### Songs — `/api/canciones`

| Method | Route | Description |
|---|---|---|
| GET | `/api/canciones/{id}` | Get song by ID |
| GET | `/api/canciones/activos` | List all active songs |
| GET | `/api/canciones/inactivos` | List all inactive songs |
| GET | `/api/canciones/todos` | List all songs |
| POST | `/api/canciones` | Create song |
| PUT | `/api/canciones/{id}` | Update song |
| DELETE | `/api/canciones/{id}` | Soft delete song |
| DELETE | `/api/canciones/{id}/fisico` | Physical delete song |
| PUT | `/api/canciones/{id}/reactivar` | Reactivate song |

> Full request and response schemas are available in the Swagger UI at `/swagger` when the API is running.

---

## Seed Data

On first startup, the application inserts the following records if the database is empty:

**Artists**

| Name | Nationality | Birth Date |
|---|---|---|
| Artista A | Argentina | 1990-05-01 |
| Artista B | España | 1985-07-12 |
| Artista C | México | 1992-03-20 |

**Songs**

| Title | Duration | Genre | Artist |
|---|---|---|---|
| Exito Musical | 5 min | Pop | Artista A |
| Rock en la Noche | 4 min | Rock | Artista B |
| Jazz Suave | 6 min | Jazz | Artista C |

> Seed logic is defined in `Disquera/Data/DbInitializer.cs`.

---

## Author

Diego Vargas — [@temps-code](https://github.com/temps-code)

---

<div align="center">
<img src="https://img.shields.io/badge/License-MIT-blue.svg?style=for-the-badge" alt="License: MIT">
</div>
