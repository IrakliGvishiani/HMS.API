using HMS.Application.Models.GuestDtos;
using HMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Contracts.Service
{
    public interface IGuestService
    {

        Task<GuestForGettingDto> CreateNewGuestAsync(Guest model);
    }
}
