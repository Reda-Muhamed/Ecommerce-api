using Ecomm.Core.Entities.User;
using Ecomm.Core.Interfaces;
using Ecomm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        // service layer will handle exceptions not the repository
        private readonly AppDbContext appContext;

        public UserRepository(AppDbContext appContext)
        {
            this.appContext = appContext;
        }
        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            await appContext.Users.AddAsync(user, ct);
        }

        public async Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken ct = default)
        {
            return await appContext.Users
                .AsNoTracking()
                .AnyAsync(u => u.EmailNormalized == normalizedEmail && !u.IsDeleted, ct);
        }


        public async Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken ct = default)
        {
            return await appContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.EmailNormalized == normalizedEmail && !u.IsDeleted, ct);
        }


        public async Task<User?> FindByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await appContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);
        }

        public Task Update(User user, CancellationToken ct = default)
        {
            appContext.Users.Update(user);
            return Task.CompletedTask;
        }

    }
}
