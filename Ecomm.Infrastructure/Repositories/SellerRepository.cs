using Ecomm.Core.Interfaces;
using Ecomm.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Ecomm.Infrastructure.Repositories
{
    internal class SellerRepository : ISellerRepository
    {
        private readonly AppDbContext appDbContext;

        public SellerRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }
        public async Task<Guid> GetByUserIdAsync(Guid UserId, CancellationToken ct)
        {
            var seller = await appDbContext.Sellers
                .FirstOrDefaultAsync(s => s.UserId == UserId, ct);
            if (seller == null)
            {
                throw new KeyNotFoundException("Seller not found for the given UserId.");
            }
            return seller.Id;
        }
    }
}
