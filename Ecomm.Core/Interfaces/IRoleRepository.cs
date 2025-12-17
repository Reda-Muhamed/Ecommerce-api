using Ecomm.Core.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Interfaces
{
    public interface IRoleRepository
    {
        public Task<Role> GetByNameAsync(string roleName,CancellationToken cancellationToken);
        public Task<Role> GetByIdAsync(Guid roleId, CancellationToken cancellationToken);
        public Task<bool> RoleExistsAsync(string roleName, CancellationToken cancellationToken);
    }
}
