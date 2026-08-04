using HMS.Application.Contracts.Persistance;
using HMS.Domain.Entities;
using HMS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Infrastructure.Persistance
{
    public class ReservationRoomRepository : RepositoryBase<ReservationRoom, ApplicationDbContext>, IReservationRoomRepository
    {
        public ReservationRoomRepository(ApplicationDbContext context) : base(context)
        {
        }
    
    }
}
