using HMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Contracts.Service
{
    public interface IAdminService
    {
        Task<int> CreateAdminAsync(Admin model);

        Task<int> DeleteAdminAsync(int id,string userId);
    }
}
