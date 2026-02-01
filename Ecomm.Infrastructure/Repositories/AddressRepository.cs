using Ecomm.Core.Entities.User;
using Ecomm.Core.Interfaces;
using Ecomm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly AppDbContext _context;

        public AddressRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Address?> GetByIdAsync(Guid addressId, CancellationToken ct)
        {
            return await _context.Addresses
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == addressId, ct);
        }
    }

}
