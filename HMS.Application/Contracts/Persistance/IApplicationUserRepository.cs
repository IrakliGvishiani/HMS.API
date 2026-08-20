using HMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace HMS.Application.Contracts.Persistance
{
    public interface IApplicationUserRepository : IRepositoryBase<ApplicationUser, DbContext>
    {
    }
}
