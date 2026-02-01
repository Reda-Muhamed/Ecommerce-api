using Ecomm.Core.Entities.Order;
using Ecomm.Core.Interfaces;
using Ecomm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext appDbContext;

        public OrderRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<bool> AddAsync(Order order, CancellationToken ct)
        {
            await appDbContext.Orders.AddAsync(order, ct);
            return true;
        }

        public async Task<Order?> GetByIdAsync(Guid orderId, CancellationToken ct)
        {
            var order =await appDbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId, ct);
            return order;
        }

        public async Task<bool> ProductHasOrdersAsync(Guid productId, CancellationToken cancellationToken)
        {
            //check if the product has orders
            var result = await appDbContext.OrderItems
                .AnyAsync(oi => oi.ProductId == productId);
            return result;
            
        }
        public async Task<bool> VariantHasOrdersAsync(Guid variantId, CancellationToken ct)
        {
            return await appDbContext.OrderItems
                .AsNoTracking()
                .AnyAsync(oi => oi.ProductVariantId == variantId, ct);
        }

    }
}
