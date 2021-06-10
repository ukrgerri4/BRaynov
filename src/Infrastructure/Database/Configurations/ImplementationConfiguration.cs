using Domain.Entities.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations
{
    public class ImplementationConfiguration : IEntityTypeConfiguration<Implementation>
    {
        public void Configure(EntityTypeBuilder<Implementation> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
