using Ecomm.Core.Entities;
using Ecomm.Core.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecomm.Infrastructure.Data.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Line1).IsRequired().HasMaxLength(500);
            builder.Property(a => a.City).IsRequired().HasMaxLength(200);
            builder.Property(a => a.PostalCode).IsRequired().HasMaxLength(50);
            builder.Property(a => a.Country).IsRequired().HasMaxLength(100);

            // Relationship
            builder.HasOne(a => a.User)
                   .WithOne(u => u.Address)
                   .HasForeignKey<Address>(a => a.UserId) // Fixed the type for HasForeignKey
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
