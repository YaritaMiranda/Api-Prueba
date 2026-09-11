# Api-Prueba

API REST desarrollada con ASP.NET Core Minimal API sobre `net8.0`.

## Requisitos

- .NET SDK 8 o superior.
- SQLite se usa mediante EF Core. No se requiere instalar un servidor de base de datos.
- Visual Studio 2022

## Como ejecutar

```powershell
dotnet restore
dotnet run --project .\Api-Prueba\Api-Prueba.csproj
```

o simplemente ejecutando desde la interfaz grafica de visual studio


## API Key

Toda llamada a los endpoints de la API debe incluir:

```text
X-API-KEY: dev-api-key-12345
```

La clave esta configurada en `Api-Prueba/appsettings.json`.

Ejemplo:

```powershell
curl -H "X-API-KEY: dev-api-key-12345" http://localhost:5000/users
```

## Base de datos

La API usa SQLite con EF Core y `IDbContextFactory<AppDbContext>`.

La base de datos se crea automaticamente al iniciar la aplicacion mediante `Database.EnsureCreatedAsync()`. El archivo generado es:

```text
Api-Prueba/user_currency.db
```

Tambien se cargan monedas iniciales si la tabla `Currencies` esta vacia:

- PYG con `RateToBase = 1`
- USD con `RateToBase = 7300`
- EUR con `RateToBase = 7900`
- BRL con `RateToBase = 1450`


## Implementado

- Minimal API en `net8.0`.
- CRUD de usuarios.
- CRUD de direcciones relacionadas a usuarios 1:N.
- Modulo de monedas y conversion de divisas.
- Seguridad por API Key con header `X-API-KEY`.
- EF Core con SQLite.
- `IDbContextFactory<AppDbContext>` registrado con `AddDbContextFactory`.
- FluentValidation para requests.
- Separacion CQRS en carpetas `Application/*/Commands` y `Application/*/Queries`.
- Procesamiento paralelo con `Task.WhenAll` en `POST /users/bulk` y busqueda paralela de monedas en `POST /currency/convert`.
- Swagger.
- Archivo JSON de prueba para bulk.

No queda funcionalidad requerida pendiente.