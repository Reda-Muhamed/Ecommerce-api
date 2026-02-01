using Ecomm.Core.DTOs;
using Ecomm.Core.Services;
using Ecomm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _context;

        public InventoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> ReserveStockAsync(Guid variantId, int quantity, CancellationToken ct)
        {
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.ProductVariantId == variantId, ct);

            if (inventory == null)
                throw new InvalidOperationException("Inventory not found");

            if (inventory.AvailableQuantity < quantity)
                throw new InvalidOperationException("Insufficient stock");

            inventory.AvailableQuantity -= quantity;
            inventory.ReservedQuantity += quantity;
            return Result<bool>.Success(true);
        }

        public async Task ReleaseStockAsync(Guid variantId, int quantity, CancellationToken ct)
        {
            var inventory = await _context.Inventories
                .FirstAsync(i => i.ProductVariantId == variantId, ct);

            inventory.AvailableQuantity += quantity;
            inventory.ReservedQuantity -= quantity;
        }

        public async Task CommitStockAsync(Guid variantId, int quantity, CancellationToken ct)
        {
            var inventory = await _context.Inventories
                .FirstAsync(i => i.ProductVariantId == variantId, ct);

            inventory.ReservedQuantity -= quantity;
        }
        public async Task ConfirmReservationAsync(Guid orderId, CancellationToken ct)
        {
            // Get order items
            var items = await _context.OrderItems
                .Where(i => i.OrderId == orderId)
                .ToListAsync(ct);

            foreach (var item in items)
            {
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.ProductVariantId == item.ProductVariantId, ct);

                if (inventory == null)
                    throw new InvalidOperationException("Inventory not found");

                if (inventory.ReservedQuantity < item.Quantity)
                    throw new InvalidOperationException("Invalid inventory reservation");

                inventory.ReservedQuantity -= item.Quantity;
                inventory.AvailableQuantity -= item.Quantity;
                inventory.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }

        public async Task ReleaseReservationAsync(Guid orderId, CancellationToken ct)
        {
            var items = await _context.OrderItems
                .Where(i => i.OrderId == orderId)
                .ToListAsync(ct);

            foreach (var item in items)
            {
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.ProductVariantId == item.ProductVariantId, ct);

                if (inventory == null)
                    continue;

                inventory.ReservedQuantity -= item.Quantity;
                inventory.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }

    }

}
