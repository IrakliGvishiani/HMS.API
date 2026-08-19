using HMS.Application.Contracts.Persistance;
using HMS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Infrastructure.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
        _context = context;
        }
        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CurrentTransaction!.CommitAsync();
        }

        public async Task RollbackTransactionAsync()
        {
           await _context.Database.CurrentTransaction!.RollbackAsync();
        }
    }
}
