using HMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Infrastructure.Data.Configurations
{
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FirstName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.LastName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.Email)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.PersonalNumber)
                   .IsRequired()
                   .HasMaxLength(11);

            builder.HasIndex(x => x.Email)
                   .IsUnique();

            builder.HasIndex(x => x.PersonalNumber)
                   .IsUnique();

            builder.Property(x => x.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasOne(x => x.ApplicationUser)
       .WithOne(x => x.Admin)
       .HasForeignKey<Admin>(x => x.ApplicationUserId);

        }
    }
}
