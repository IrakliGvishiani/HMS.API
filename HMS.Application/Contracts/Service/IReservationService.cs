using HMS.Application.Models.ReservationDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Contracts.Service
{
    public interface IReservationService
    {
        Task<ReservationForGettingDto> CreateReservationAsync(ReservationForCreatingDto model,string userId,string userRole);

        Task<int> UpdateReservationAsync(ReservationForUpdatingDto model,string userId,string userRole);

        Task<int> DeleteReservationAsync(int id,string userId,string userRole);
        Task<IEnumerable<ReservationForGettingDto>> SearchReservationsAsync(ReservationSearchDto model,string userId,string userRole);

        Task<ReservationForGettingDto> GetReservationByIdAsync(int id, string userId, string userRole);
    }
}
