using Ecomm.Core.Interfaces;
using Ecomm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Repositories
{
    public class CategoryRepository: ICategoryRepository
    {
        private readonly AppDbContext appDbContext;

       public  CategoryRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }
        public async Task<bool> ExistsAsync(Guid categoryId, CancellationToken cancellationToken)
        {
            return await appDbContext.Categories
                .AsNoTracking()
                .AnyAsync(c => c.Id == categoryId, cancellationToken);
        }
    }
}
