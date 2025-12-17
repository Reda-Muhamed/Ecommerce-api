using Ecomm.Core.Entities.User;
using Ecomm.Core.Interfaces;
using Ecomm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecomm.Infrastructure.Repositories
{
    public class RoleRepository(AppDbContext context) : IRoleRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Role?> GetByIdAsync(Guid roleId, CancellationToken ct = default)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == roleId, ct);
        }

        public async Task<Role?> GetByNameAsync(string roleName, CancellationToken ct = default)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName, ct);
        }

        public Task<bool> RoleExistsAsync(string roleName, CancellationToken ct = default)
        {
            return _context.Roles
                .AnyAsync(r => r.Name == roleName, ct);
        }
    }
}
