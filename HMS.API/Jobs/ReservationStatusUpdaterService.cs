using HMS.Application.Contracts.Persistance;
using HMS.Domain.Enum;

namespace HMS.API.Jobs
{
    public class ReservationStatusUpdaterService(
        IServiceScopeFactory serviceScopeFactory
       
        ) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceScopeFactory.CreateScope();

                var now = DateTime.UtcNow;

                var reservationRepository = scope.ServiceProvider.GetRequiredService<IReservationRepository>();

                /// RESERVED TO ACTIV

                var (reservedReservations, _) = await reservationRepository.GetAllAsync(
                    filter: x => 
                    x.Status == ReservationStatus.Reserved && 
                    x.CheckInDate <= now &&
                    x.CheckOutDate > now,

                    tracking:true
                    );

                foreach (var item in reservedReservations)
                {
                    item.Status = ReservationStatus.Active;  
                }

                ///ACTIVE TO COMPLETED
                var (activeReservations, _) = await reservationRepository.GetAllAsync(
                    filter: x => 
                    x.Status == ReservationStatus.Active &&
                    x.CheckOutDate <= now,
                    tracking:true
                    );

                foreach (var item in activeReservations)
                {
                    item.Status = ReservationStatus.Completed;
                }

                await reservationRepository.SaveAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(1),stoppingToken);
            }
        }
    }
}
