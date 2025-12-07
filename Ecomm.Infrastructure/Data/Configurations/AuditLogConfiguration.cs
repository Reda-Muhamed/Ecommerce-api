using Ecomm.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecomm.Infrastructure.Data.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.EntityName).IsRequired().HasMaxLength(200);
            builder.Property(a => a.ActionType).IsRequired().HasMaxLength(50);
            builder.Property(a => a.CreatedAt).IsRequired();
        }
    }
}
