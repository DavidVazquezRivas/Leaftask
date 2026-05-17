using Microsoft.EntityFrameworkCore;
using Modules.Projects.Domain.Entities.Field;

namespace Modules.Projects.DrivenInfrastructure.Persistence.Seeding;

public static class FieldTypeSeeder
{
    private static readonly FieldType[] FieldTypes =
    [
        new(new Guid("c1d2e3f4-0001-0000-0000-000000000001"), "Texto", "Campo de texto de una línea"),
        new(new Guid("c1d2e3f4-0002-0000-0000-000000000001"), "Número", "Valores numéricos únicamente"),
        new(new Guid("c1d2e3f4-0003-0000-0000-000000000001"), "Fecha", "Selector de fecha"),
        new(new Guid("c1d2e3f4-0004-0000-0000-000000000001"), "Selección", "Opción única desplegable"),
        new(new Guid("c1d2e3f4-0005-0000-0000-000000000001"), "Selección Múltiple", "Múltiples opciones"),
        new(new Guid("c1d2e3f4-0006-0000-0000-000000000001"), "Persona", "Seleccionar un miembro del equipo"),
        new(new Guid("c1d2e3f4-0007-0000-0000-000000000001"), "Casilla de Verificación", "Checkbox verdadero/falso"),
        new(new Guid("c1d2e3f4-0008-0000-0000-000000000001"), "Enlace", "URL o enlace web")
    ];

    public static async Task SeedAsync(ProjectsDbContext dbContext, CancellationToken cancellationToken = default)
    {
        bool alreadySeeded = await dbContext.FieldTypes
            .AnyAsync(ft => ft.Name == "Texto", cancellationToken);

        if (alreadySeeded)
        {
            return;
        }

        await dbContext.FieldTypes.AddRangeAsync(FieldTypes, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
