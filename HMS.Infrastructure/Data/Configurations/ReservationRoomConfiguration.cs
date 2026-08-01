using HMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Infrastructure.Data.Configurations
{
    public class ReservationRoomConfiguration : IEntityTypeConfiguration<ReservationRoom>
    {
        public void Configure(EntityTypeBuilder<ReservationRoom> builder)
        {
            builder.HasKey(x => new
            {
                x.ReservationId,
                x.RoomId
            });

            builder.HasOne(x => x.Reservation)
                   .WithMany(x => x.ReservationRooms)
                   .HasForeignKey(x => x.ReservationId);

            builder.HasOne(x => x.Room)
                   .WithMany(x => x.ReservationRooms)
                   .HasForeignKey(x => x.RoomId);
        }
    }
}
