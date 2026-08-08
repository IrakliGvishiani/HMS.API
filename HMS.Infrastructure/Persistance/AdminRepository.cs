using HMS.Application.Contracts.Persistance;
using HMS.Domain.Entities;
using HMS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Infrastructure.Persistance
{
    public class AdminRepository : RepositoryBase<Admin, ApplicationDbContext>, IAdminRepository
    {
        public AdminRepository(ApplicationDbContext context) : base(context)
        {
        }
    
    }
}
