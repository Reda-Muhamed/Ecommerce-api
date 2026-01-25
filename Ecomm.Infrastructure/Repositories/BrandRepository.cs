using Ecomm.Core.Interfaces;
using Ecomm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly AppDbContext appDbContext;

        public BrandRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }
        public async Task<bool> ExistsAsync(Guid brandId, CancellationToken cancellationToken)
        {
            return await appDbContext.Brands.AnyAsync(b => b.Id == brandId);
        }
    }
}
