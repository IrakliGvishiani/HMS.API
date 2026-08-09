using HMS.Application.Models.AuthDtos;
using HMS.Application.Models.ManagerDtos;
using HMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Contracts.Service
{
    public interface IManagerService
    {

        Task<int> CreateManagerAsync(Manager model);

        Task<int> DeleteManagerAsync(int id);

        Task<IEnumerable<ManagerListForGettingDto>> GetManagersAsync();

        Task<int> UpdateManagerAsync(ManagerForUpdatingDto model);
    }
}
