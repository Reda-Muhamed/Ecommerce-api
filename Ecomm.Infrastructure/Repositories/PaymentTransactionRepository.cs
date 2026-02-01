using Ecomm.Core.Entities;
using Ecomm.Core.Interfaces;
using Ecomm.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Repositories
{
    public class PaymentTransactionRepository : IPaymentTransactionRepository
    {
        private readonly AppDbContext _context;

        public PaymentTransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PaymentTransaction transaction, CancellationToken ct)
        {
            await _context.PaymentTransactions.AddAsync(transaction, ct);
        }
    }

}
