using HMS.Application.Contracts.Persistance;
using HMS.Domain.Entities;
using HMS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Infrastructure.Persistance
{
    public class ManagerRepository : RepositoryBase<Manager, ApplicationDbContext>, IManagerRepository
    {
        public ManagerRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
