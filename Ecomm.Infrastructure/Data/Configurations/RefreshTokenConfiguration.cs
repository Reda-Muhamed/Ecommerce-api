using Ecomm.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecomm.Infrastructure.Data.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TokenHash).IsRequired().HasMaxLength(256);

            builder.Property(x => x.ExpiresAt).IsRequired();

            builder.Property(x => x.RevokedAt);

            builder.Property(x => x.CreatedAt).IsRequired();

            //relationships
            builder.HasOne(r=>r.User)
                .WithMany(u=>u.RefreshTokens)
                .HasForeignKey(r=>r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x=> new {x.UserId,x.ExpiresAt});

        }
    }
}