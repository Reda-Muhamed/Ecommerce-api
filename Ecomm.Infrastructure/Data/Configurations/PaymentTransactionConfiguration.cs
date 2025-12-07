using Ecomm.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecomm.Infrastructure.Data.Configurations
{
    public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            builder.ToTable("PaymentTransactions");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount).HasColumnType("decimal(18,4)");
            builder.Property(p => p.TransactionId).IsRequired().HasMaxLength(200);
            builder.HasOne(p => p.Order)
                .WithMany(o => o.PaymentTransactions)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
