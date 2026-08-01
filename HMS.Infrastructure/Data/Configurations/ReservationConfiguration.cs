using HMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Infrastructure.Data.Configurations
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CheckInDate)
                .IsRequired();

            builder.Property(x => x.CheckOutDate)
                .IsRequired();

            builder.HasOne(x => x.Guest)
                .WithMany(x => x.Reservations)
                .HasForeignKey(x => x.GuestId);

            builder.HasMany(x => x.ReservationRooms)
                .WithOne(x => x.Reservation)
                .HasForeignKey(x => x.ReservationId);
        }
    }
}
