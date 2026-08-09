using HMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Infrastructure.Data.Configurations
{
    public class GuestConfiguration : IEntityTypeConfiguration<Guest>
    {
        public void Configure(EntityTypeBuilder<Guest> builder)
        {

            builder.HasKey(x => x.Id);

            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.LastName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.PersonalNumber)
                .IsRequired()
                .HasMaxLength(11);

            builder.HasIndex(x => x.PersonalNumber)
                .IsUnique();

            builder.Property(x => x.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Email)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(x => x.PhoneNumber).IsUnique();

            builder.HasMany(x => x.Reservations)
                .WithOne(x => x.Guest)
                .HasForeignKey(x => x.GuestId);

            builder.HasIndex(x => x.Email)
                  .IsUnique();

            builder.HasOne(x => x.ApplicationUser)
      .WithOne(x => x.Guest)
      .HasForeignKey<Guest>(x => x.ApplicationUserId);
        }
    }
}
