using Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations
{
    public class AuditEntryExtendedConfiguration : IEntityTypeConfiguration<AuditEntryExtended>
    {
        public void Configure(EntityTypeBuilder<AuditEntryExtended> builder)
        {
            builder.Property(x => x.RequestId).HasMaxLength(36);
        }
    }
}
