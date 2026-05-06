using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Projects.Domain.Entities.Owner;

namespace Modules.Projects.DrivenInfrastructure.Entities.Owner;

internal sealed class OrganizationReadModelEntityTypeConfiguration : IEntityTypeConfiguration<OrganizationReadModel>
{
    public void Configure(EntityTypeBuilder<OrganizationReadModel> builder)
    {
        builder.ToTable("organizations_read_models");

        builder.HasKey(organization => organization.Id);
    }
}
