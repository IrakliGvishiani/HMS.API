using HMS.Application.Models.ReservationDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Contracts.Service
{
    public interface IReservationService
    {
        Task<ReservationForGettingDto> CreateReservationAsync(ReservationForCreatingDto model,string userId);

        Task<int> UpdateReservationAsync(ReservationForUpdatingDto model);

        Task<int> DeleteReservationAsync(int id,string userId);
        Task<IEnumerable<ReservationForGettingDto>> SearchReservationsAsync(ReservationSearchDto model);
    }
}
