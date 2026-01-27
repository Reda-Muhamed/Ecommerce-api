using Ecomm.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Interfaces
{
    public interface IAttributeValueRepository
    {
        Task<bool> ExistsAsync(Guid attributeId, string value, CancellationToken ct);
        Task AddAsync(AttributeValue value, CancellationToken ct);
        Task UpdateAsync(AttributeValue value, CancellationToken ct);
        Task<AttributeValue?> GetAsync(Guid id, CancellationToken ct);
        Task DeleteAsync(AttributeValue value, CancellationToken ct);
    }

}
