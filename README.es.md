<div align="center">

<h1>Sistema de Gestion de Canciones y Artistas</h1>

<p><strong>API REST para gestionar un catalogo musical con artistas y canciones, construida con ASP.NET Core 8 y Entity Framework Core.</strong></p>

<p>
  <img src="https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8">
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" alt="C#">
  <img src="https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white" alt="SQL Server">
  <img src="https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black" alt="Swagger">
</p>

</div>

---

Leelo en: [English](README.md) | **Espanol**

**Proyecto Academico**
Universidad Privada Domingo Savio — Ing. de Sistemas
Materia: Sistemas de Informacion II — Junio 2025

---

## Tabla de Contenidos

- [Descripcion](#descripcion)
- [Stack](#stack)
- [Arquitectura](#arquitectura)
- [Instalacion](#instalacion)
- [Variables de Entorno](#variables-de-entorno)
- [Endpoints](#endpoints)
- [Datos de Prueba](#datos-de-prueba)
- [Autor](#autor)

---

## Descripcion

API REST backend que gestiona el catalogo musical de una disquera. Administra artistas y canciones con operaciones CRUD completas, soporte para borrado logico e integridad referencial entre entidades.

- Crear, leer, actualizar y eliminar artistas y canciones
- Borrado logico: los registros se marcan como inactivos en lugar de eliminarse fisicamente
- Borrado fisico: eliminacion completa del registro cuando se requiere
- Reactivacion de registros eliminados logicamente
- Las canciones solo pueden asignarse a artistas activos
- Endpoints separados para listar registros activos, inactivos o todos
- Timestamps en UTC registrados automaticamente en cada registro
- Respuestas JSON estructuradas con detalle de errores de validacion
- Swagger UI disponible en tiempo de ejecucion para explorar la API de forma interactiva

> CORS esta configurado para permitir cualquier origen, lo que permite el consumo directo desde un frontend separado.

---

## Stack

| Categoria | Tecnologia | Version |
|---|---|---|
| Lenguaje | C# | 12 |
| Framework | ASP.NET Core | 8.0 |
| ORM | Entity Framework Core | 8.0.16 |
| Base de datos | Microsoft SQL Server Express | — |
| Documentacion API | Swashbuckle (Swagger) | 6.6.2 |

---

## Arquitectura

```
Disquera/
├── Controllers/
│   ├── ArtistasController.cs       # Endpoints REST de artistas
│   └── CancionesController.cs      # Endpoints REST de canciones
├── Models/
│   ├── Artista.cs                  # Entidad Artista
│   └── Cancion.cs                  # Entidad Cancion
├── Dtos/
│   ├── Artistas/                   # DTOs de artistas (crear, actualizar, listar, detalle)
│   └── Canciones/                  # DTOs de canciones (crear, actualizar, listar, detalle)
├── Data/
│   ├── DisqueraContext.cs          # DbContext de EF Core
│   └── DbInitializer.cs            # Sembrado de la base de datos al iniciar
├── Migrations/                     # Historial de migraciones de EF Core
├── appsettings.json                # Configuracion de la aplicacion y cadena de conexion
└── Program.cs                      # Registro de servicios y pipeline de middleware
```

**Modelo de datos**

```
Artista (1) ──── (N) Cancion
```

| Entidad | Campos |
|---|---|
| Artista | id, nombre, nacionalidad, fechaNacimiento, isActive, createdAt, updatedAt |
| Cancion | id, titulo, duracion (min), genero, artistaId (FK), isActive, createdAt, updatedAt |

> El comportamiento de eliminacion es `RESTRICT`: un artista no puede eliminarse si tiene canciones asociadas.

---

## Instalacion

**Requisitos previos**

- .NET 8 SDK
- SQL Server Express ejecutandose localmente en `localhost\SQLEXPRESS`

**Pasos**

1. Clonar el repositorio:
   ```bash
   git clone https://github.com/temps-code/Sistema-de-gestion-de-canciones-y-artistas---Backend.git
   cd Sistema-de-gestion-de-canciones-y-artistas---Backend
   ```

2. Actualizar la cadena de conexion en `Disquera/appsettings.json` con los datos de tu instancia local de SQL Server.

3. Aplicar la migracion de base de datos:
   ```bash
   cd Disquera
   dotnet ef database update
   ```

4. Ejecutar la API:
   ```bash
   dotnet run
   ```

5. Abrir Swagger UI en el navegador:
   ```
   http://localhost:5091/swagger
   ```

> La aplicacion inserta datos de prueba al iniciar por primera vez si la base de datos esta vacia.

---

## Variables de Entorno

La cadena de conexion esta definida en `Disquera/appsettings.json`. Actualizala segun tu configuracion local de SQL Server:

```json
{
  "ConnectionStrings": {
    "DisqueraDb": "Server=localhost\\SQLEXPRESS;Database=DisqueraDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

| Parametro | Descripcion |
|---|---|
| `Server` | Nombre de la instancia de SQL Server |
| `Database` | Nombre de la base de datos (se crea automaticamente en el primer arranque) |
| `Trusted_Connection` | Usar autenticacion de Windows |
| `TrustServerCertificate` | Requerido para certificados autofirmados locales |

---

## Endpoints

Todas las respuestas siguen esta estructura:

```json
{
  "success": true,
  "message": "...",
  "data": {},
  "errors": {}
}
```

### Artistas — `/api/artistas`

| Metodo | Ruta | Descripcion |
|---|---|---|
| GET | `/api/artistas/{id}` | Obtener artista por ID |
| GET | `/api/artistas/activos` | Listar todos los artistas activos |
| GET | `/api/artistas/inactivos` | Listar todos los artistas inactivos |
| GET | `/api/artistas/todos` | Listar todos los artistas |
| POST | `/api/artistas` | Crear artista |
| PUT | `/api/artistas/{id}` | Actualizar artista |
| DELETE | `/api/artistas/{id}` | Borrado logico del artista |
| DELETE | `/api/artistas/{id}/fisico` | Borrado fisico del artista |
| PUT | `/api/artistas/{id}/reactivar` | Reactivar artista |

### Canciones — `/api/canciones`

| Metodo | Ruta | Descripcion |
|---|---|---|
| GET | `/api/canciones/{id}` | Obtener cancion por ID |
| GET | `/api/canciones/activos` | Listar todas las canciones activas |
| GET | `/api/canciones/inactivos` | Listar todas las canciones inactivas |
| GET | `/api/canciones/todos` | Listar todas las canciones |
| POST | `/api/canciones` | Crear cancion |
| PUT | `/api/canciones/{id}` | Actualizar cancion |
| DELETE | `/api/canciones/{id}` | Borrado logico de la cancion |
| DELETE | `/api/canciones/{id}/fisico` | Borrado fisico de la cancion |
| PUT | `/api/canciones/{id}/reactivar` | Reactivar cancion |

> Los esquemas completos de request y response estan disponibles en Swagger UI en `/swagger` cuando la API esta en ejecucion.

---

## Datos de Prueba

Al iniciar por primera vez, la aplicacion inserta los siguientes registros si la base de datos esta vacia:

**Artistas**

| Nombre | Nacionalidad | Fecha de Nacimiento |
|---|---|---|
| Artista A | Argentina | 1990-05-01 |
| Artista B | Espana | 1985-07-12 |
| Artista C | Mexico | 1992-03-20 |

**Canciones**

| Titulo | Duracion | Genero | Artista |
|---|---|---|---|
| Exito Musical | 5 min | Pop | Artista A |
| Rock en la Noche | 4 min | Rock | Artista B |
| Jazz Suave | 6 min | Jazz | Artista C |

> La logica de sembrado esta definida en `Disquera/Data/DbInitializer.cs`.

---

## Autor

Diego Vargas — [@temps-code](https://github.com/temps-code)

---

<div align="center">
<img src="https://img.shields.io/badge/License-MIT-blue.svg?style=for-the-badge" alt="License: MIT">
</div>
