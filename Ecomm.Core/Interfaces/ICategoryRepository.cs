using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Interfaces
{
    public interface ICategoryRepository
    {
        public Task<bool> ExistsAsync(Guid categoryId, CancellationToken cancellationToken);
    }
}
