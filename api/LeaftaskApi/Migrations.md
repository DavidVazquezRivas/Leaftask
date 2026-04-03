# Migraciones EF Core

## Crear migración

```bash id="2j9g1k"
dotnet ef migrations add {MigrationName} --project {InfrastructureProject} --startup-project {StartupProject} --context {DbContext} --output-dir {MigrationsPath}
```

### Ejemplo

```bash id="f0r8zm"
dotnet ef migrations add InitialUserSchema \
  --project Modules.User.DrivenInfrastructure \
  --startup-project Api.Host \
  --context UserDbContext \
  --output-dir Persistence/Migrations
```

## Aplicación

Las migraciones se aplican automáticamente al iniciar la aplicación mediante la clase `MigrationDatabaseExtensions`.
