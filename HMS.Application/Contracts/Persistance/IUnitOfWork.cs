using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Contracts.Persistance
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
