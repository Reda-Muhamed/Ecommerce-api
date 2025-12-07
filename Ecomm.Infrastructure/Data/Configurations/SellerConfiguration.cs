using Ecomm.Core.Entities;
using Ecomm.Core.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecomm.Infrastructure.Data.Configurations
{
    public class SellerConfiguration : IEntityTypeConfiguration<Seller>
    {
        public void Configure(EntityTypeBuilder<Seller> builder)
        {
            builder.ToTable("Sellers");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name).IsRequired().HasMaxLength(200);

            builder.Property(s => s.Rating).HasColumnType("decimal(3,2)");

            builder.HasOne(s => s.User)
                .WithMany() // user->seller optional mapping
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
