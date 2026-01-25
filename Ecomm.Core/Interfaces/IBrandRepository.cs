using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Interfaces
{
    public interface IBrandRepository
    {
        public Task<bool> ExistsAsync(Guid brandId,CancellationToken cancellationToken);
    }
}
