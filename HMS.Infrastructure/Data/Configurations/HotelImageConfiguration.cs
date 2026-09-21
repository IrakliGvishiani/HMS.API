using HMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Infrastructure.Data.Configurations
{
    public class HotelImageConfiguration : IEntityTypeConfiguration<HotelImage>
    {
        public void Configure(EntityTypeBuilder<HotelImage> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ImageUrl)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(x => x.ImagePublicId)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.HasOne(x => x.Hotel)
                   .WithMany(x => x.HotelImages)
                   .HasForeignKey(x => x.HotelId);
        }
    
    
    }
}
