# LeaftaskApi — Guía para Claude

## Visión general

Monolito modular en **.NET 10 / ASP.NET Core** con PostgreSQL. Tres módulos de dominio: **Users**, **Organizations**, **Projects**. Base URL de la API: `api/v1/`.

El proyecto sigue **Clean Architecture + DDD + CQRS** dentro de cada módulo, y usa el **patrón Outbox** para comunicación entre módulos via eventos de integración.

---

## Tech Stack

| Tecnología | Versión | Uso |
|-----------|---------|-----|
| .NET / ASP.NET Core | 10 | Framework principal |
| Entity Framework Core + Npgsql | 10.0.5 / 10.0.1 | ORM + PostgreSQL |
| MediatR | 14.1.0 | CQRS, dispatch de eventos |
| FluentValidation | 12.1.1 | Validación de requests |
| Quartz.NET | 3.17.1 | Jobs para procesar el outbox |
| Serilog + Seq | 10.0.0 / sink 9.0.0 | Logging estructurado |
| xUnit v3 + NSubstitute + FluentAssertions | 3.2.2 / 5.3.0 / 8.9.0 | Tests |

Versiones NuGet centralizadas en [`Directory.Packages.props`](Directory.Packages.props).

---

## Estructura de un módulo (5 capas)

```
Modules.{Module}.Domain
    Entities/           → Aggregates, entidades, value objects, enums
    Events/             → XyzDomainEvent (sealed record : IDomainEvent)
    Repositories/       → Interfaces de repositorios
    Errors/             → Errores estáticos de dominio

Modules.{Module}.Application
    {Feature}/          → Command.cs, CommandHandler.cs, CommandValidator.cs
    {Feature}/          → Query.cs, QueryHandler.cs, QueryValidator.cs, IQueryService.cs
    Events/             → {Module}EventMapper.cs (DomainEvent → IntegrationEvent)
    Authorization/      → OrganizationPermissionBehavior (si aplica)

Modules.{Module}.Integration
    XyzIntegrationEvent.cs     → Contratos públicos para otros módulos
    IXyzService.cs             → Interfaces de servicio cross-módulo

Modules.{Module}.DrivenInfrastructure
    Persistence/        → {Module}DbContext.cs, Migrations/, Schema.cs, Factory
    Entities/           → IEntityTypeConfiguration<T> por entidad
    Repositories/       → Implementaciones de repositorios
    Queries/            → Implementaciones de IQueryService (cursor pagination)
    Authorization/      → Implementaciones de permission checkers (si aplica)

Modules.{Module}.DrivingInfrastructure
    Controllers/        → {Feature}Controller.cs (hereda ApiBaseController)
    Models/Requests/    → DTOs de request HTTP
    Jobs/               → {Module}OutboxJob.cs
    Subscribers/        → IntegrationEventHandler<T> por evento recibido
    Services/           → Servicios expuestos a otros módulos (ej: PermissionService)
    Setup/              → DependencyInjection.cs, {Module}ModuleInitialization.cs
```

---

## Patrones clave

### CQRS con MediatR

**Command** (modifica estado):
```csharp
public sealed record CreateXyzCommand(string Name) : ICommand<Result<XyzResponse>>;

public sealed class CreateXyzCommandHandler(IXyzRepository repository)
    : ICommandHandler<CreateXyzCommand, Result<XyzResponse>>
{
    public async Task<Result<XyzResponse>> Handle(CreateXyzCommand command, CancellationToken ct)
    {
        // ...
        return Result.Success(new XyzResponse(...));
    }
}
```

**Query** (solo lectura):
```csharp
public sealed record GetXyzQuery(Guid Id) : IQuery<Result<XyzResponse>>;
```

**Pipeline behaviors** (se ejecutan en orden):
1. `LoggingBehavior` — timing y tracing
2. `ValidationBehavior` — FluentValidation
3. `OrganizationPermissionBehavior` — solo si el request tiene `[RequireOrganizationPermission]`

### Domain Events → Integration Events (flujo completo)

```
Aggregate.Raise(new XyzDomainEvent(...))
    ↓ SaveChangesAsync() en AppDbContext
    ↓ DomainEventsDispatcher despacha via MediatR
    ↓ EventMapper.Map(domainEvent) → XyzIntegrationEvent
    ↓ Serializado en tabla OutboxMessages (JSON)
    ↓ Quartz job cada 5s lee batch de OutboxMessages
    ↓ Publica IntegrationEvent via MediatR IPublisher
    ↓ IntegrationEventHandler<T> en módulo receptor
        → verifica InboxMessages (deduplicación)
        → ejecuta lógica
        → guarda en InboxMessages
```

### Result Pattern

Nunca lanzar excepciones para fallos de negocio:

```csharp
// Éxito
return Result.Success(response);
return Result.Success();

// Fallo
return Result.Failure<XyzResponse>(XyzErrors.NotFound);
```

Errores definidos como statics en `{Module}Errors.cs`:
```csharp
public static class XyzErrors
{
    public static readonly Error NotFound = new("Xyz.NotFound", "Xyz not found", 404);
    public static readonly Error AlreadyExists = new("Xyz.AlreadyExists", "Xyz already exists", 409);
}
```

### Read Models (sincronización entre módulos)

Cada módulo mantiene sus propias copias (read models) de entidades de otros módulos:
- `OrganizationReadModel` y `UserReadModel` en el módulo Projects
- `UserReadModel` en el módulo Organizations
- Se actualizan vía integration event handlers (eventual consistency)
- Los handlers son **idempotentes**: comprueban existencia antes de crear/borrar

### Autorización por permisos de organización

```csharp
[RequireOrganizationPermission("Configure Organization")]
public sealed record DeleteOrganizationCommand(Guid OrganizationId)
    : ICommand<Result>, IOrganizationPermissionRequest;
```

El behavior `OrganizationPermissionBehavior` intercepta cualquier request que:
1. Implemente `IOrganizationPermissionRequest` (tiene `OrganizationId`)
2. Tenga el attribute `[RequireOrganizationPermission]`

Niveles de retorno: `Full`, `Supervised`, `Denied`, `MembershipRequired`, `PermissionNotFound`, `OrganizationNotFound`.

### Paginación cursor-based

```csharp
public sealed record GetMyXyzQuery(int Limit, string? Cursor, string? Sort)
    : IQuery<Result<PaginatedResult<XyzDto>>>, IPaginatedQuery<XyzDto>;
```

Sort fields definidos en diccionario estático en el QueryService. Siempre incluye ID como sort secundario para consistencia.

---

## Cómo añadir una nueva feature

### Nueva command/query en módulo existente

1. **Domain** — añadir método al aggregate si aplica; `Raise(new XyzDomainEvent(...))` si genera evento
2. **Domain/Events** — `XyzDomainEvent.cs` si es nuevo
3. **Application/{Feature}/** — `XyzCommand.cs` + `XyzCommandHandler.cs` + `XyzCommandValidator.cs`
4. **Application/Events/EventMapper** — añadir case al switch si hay nuevo DomainEvent
5. **Integration/** — `XyzIntegrationEvent.cs` si otros módulos deben reaccionar
6. **DrivenInfrastructure/Repositories/** — implementar nuevo método si hace falta
7. **DrivingInfrastructure/Controllers/** — endpoint HTTP con `HandleResult(await Sender.Send(...))`
8. **DrivingInfrastructure/Subscribers/** — `XyzIntegrationEventHandler` si el módulo recibe el evento

### Nuevo módulo completo

1. Crear 5 proyectos (`.csproj`) siguiendo el naming `Modules.{Module}.{Layer}`
2. Añadir al `LeaftaskApi.slnx`
3. `DrivenInfrastructure`: DbContext heredando de `AppDbContext`, Schema.cs, Factory, migraciones
4. `DrivingInfrastructure/Setup/DependencyInjection.cs`: método `Add{Module}Module(IServiceCollection, IConfiguration)`
5. Registrar en [`Api.Host/Program.cs`](Api.Host/Program.cs): `builder.Services.Add{Module}Module(...)`
6. Añadir `ApplyMigrationsAsync` en [`MigrationDatabaseExtensions.cs`](Api.Host/Infrastructure/DatabaseExtensions/MigrationDatabaseExtensions.cs)

---

## Convenciones de nombrado

| Elemento | Convención | Ejemplo |
|---------|-----------|---------|
| Tablas DB | snake_case | `organization_role_permissions` |
| Columnas DB | snake_case | `created_at` |
| Schema por módulo | string constante | `"organization"`, `"project"`, `"user"` |
| Domain events | `XyzDomainEvent` sealed record | `OrganizationDeletedDomainEvent` |
| Integration events | `XyzIntegrationEvent` sealed record | `OrganizationDeletedIntegrationEvent` |
| Commands | `VerbNounCommand` | `DeleteOrganizationCommand` |
| Queries | `GetNounQuery` | `GetMyProjectsQuery` |
| Errores | `"Module.Entity.Problem"` | `"Organization.Permission.Denied"` |
| Carpetas feature | PascalCase por caso de uso | `Management/Delete/` |

---

## Tests

- Un proyecto por capa: `Modules.{Module}.Domain.UnitTests`, `Modules.{Module}.Application.UnitTests`
- **Builders** para datos de test (ej: `CreateOrganizationCommandTestBuilder`)
- **NSubstitute** para mocks, **FluentAssertions** para assertions
- Patrón: Arrange / Act / Assert explícito
- Probar tanto el happy path como los casos de error

---

## Archivos críticos de referencia

| Archivo | Propósito |
|---------|-----------|
| [`BuildingBlocks.Domain/Entities/Entity.cs`](BuildingBlocks.Domain/Entities/Entity.cs) | Base de aggregates con `Raise()` |
| [`BuildingBlocks.Domain/Result/Result.cs`](BuildingBlocks.Domain/Result/Result.cs) | Result pattern |
| [`BuildingBlocks.Application/Behaviors/`](BuildingBlocks.Application/Behaviors/) | Pipeline behaviors (Logging, Validation) |
| [`BuildingBlocks.DrivenInfrastructure/Persistence/AppDbContext.cs`](BuildingBlocks.DrivenInfrastructure/Persistence/AppDbContext.cs) | SaveChanges + outbox |
| [`BuildingBlocks.DrivingInfrastructure/Controllers/ApiBaseController.cs`](BuildingBlocks.DrivingInfrastructure/Controllers/ApiBaseController.cs) | `HandleResult()` helper |
| [`BuildingBlocks.DrivingInfrastructure/Jobs/Outbox/OutboxJob.cs`](BuildingBlocks.DrivingInfrastructure/Jobs/Outbox/OutboxJob.cs) | Base del job de outbox |
| [`BuildingBlocks.DrivingInfrastructure/Events/IntegrationEventHandler.cs`](BuildingBlocks.DrivingInfrastructure/Events/IntegrationEventHandler.cs) | Base para subscribers con inbox |
| [`Api.Host/Program.cs`](Api.Host/Program.cs) | Bootstrap y registro de módulos |
| [`Directory.Packages.props`](Directory.Packages.props) | Versiones NuGet centralizadas |

---

## Base de datos

- **PostgreSQL** local: `Host=localhost;Port=5432;Database=leaftask;Username=postgres;Password=password`
- Migraciones aplicadas al arrancar (`ApplyAllMigrationsAsync` con retry x10)
- Seeding solo en desarrollo (`isDevelopment`)
- Cada módulo tiene su propio schema aislado

### Añadir migración

```bash
# Desde la raíz de la solución
dotnet ef migrations add MigrationName \
  --project Modules.{Module}.DrivenInfrastructure \
  --startup-project Api.Host \
  --context {Module}DbContext
```
