using Ecomm.Core.Entities;
using Ecomm.Core.Entities.User;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

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
            //Database Seeding
            var createdAt = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);

            builder.HasData(
                new Role
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Customer",
                    CreatedAt = createdAt
                },
                new Role
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Seller",
                    CreatedAt = createdAt
                },
                new Role
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Admin",
                    CreatedAt = createdAt
                }
            );

        }
    }
}
