using Ecomm.Core.Entities;
using Ecomm.Core.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecomm.Infrastructure.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name).IsRequired().HasMaxLength(100);

            builder.Property(r => r.RowVersion)
                   .IsRowVersion();

            builder.HasIndex(r => r.Name).IsUnique();

            // Relationship: Role (1) -> Users (many)
            builder.HasMany(r => r.Users)
               .WithOne(ur => ur.Role)
               .HasForeignKey(ur => ur.RoleId)
               .OnDelete(DeleteBehavior.Restrict);

            }
    }
}
