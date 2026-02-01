using Ecomm.Core.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Interfaces
{
    public interface IAddressRepository
    {
        Task<Address?> GetByIdAsync(Guid id, CancellationToken ct);
    }
}
