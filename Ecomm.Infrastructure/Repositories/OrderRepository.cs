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
