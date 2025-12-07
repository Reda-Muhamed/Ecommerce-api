using Ecomm.Core.Entities.Cart;
using Ecomm.Core.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Data.Configurations
{
    public class UserConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u=>u.Email).IsRequired().HasMaxLength(256);

            builder.Property(u=>u.EmailNormalized).IsRequired().HasMaxLength(256);

            // Unique index on normalized email
            builder.HasIndex(u => u.EmailNormalized).IsUnique();

            builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(256);


            // soft-delete filter
            builder.HasQueryFilter(u => !u.IsDeleted);

            //Row version
            builder.Property(u => u.RowVersion).IsRowVersion();

            // Orders (one-to-many)
            builder.HasMany(u => u.Orders)
                .WithOne(o => o.User!)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // RefreshTokens (one-to-many)
            builder.HasMany(u => u.RefreshTokens)
                   .WithOne(rt => rt.User!)
                   .HasForeignKey(rt => rt.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Reviews (one-to-many)
            builder.HasMany(u => u.Reviews)
                   .WithOne(r => r.User!)
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Carts (one-to-many) because user can have multiple carts
            builder.HasMany(u => u.Carts)
                   .WithOne(c => c.User)
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Role (many to one)
            builder.HasOne(u => u.Role)
                   .WithMany(r=>r.Users)
                   .HasForeignKey(u=>u.RoleId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
