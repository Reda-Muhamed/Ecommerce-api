using Ecomm.Core.Services;
using Ecomm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

public class UnitOfWork : IUnitOfWork
{
    private IDbContextTransaction? transaction;
    private readonly AppDbContext context;

    public UnitOfWork(AppDbContext context)
    {
        this.context = context;
    }

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (transaction != null) return;
        transaction = await context.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (transaction == null) return;
        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        await transaction.DisposeAsync();
        transaction = null;
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (transaction == null) return;
        await transaction.RollbackAsync(ct);
        await transaction.DisposeAsync();
        transaction = null;
    }

   
    public void Dispose()
    {
        transaction?.Dispose();
    }
}
