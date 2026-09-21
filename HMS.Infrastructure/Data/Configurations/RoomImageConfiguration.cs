using HMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Infrastructure.Data.Configurations
{
    public class RoomImageConfiguration : IEntityTypeConfiguration<RoomImage>
    {
        public void Configure(EntityTypeBuilder<RoomImage> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ImageUrl)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(x => x.ImagePublicId)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.HasOne(x => x.Room)
                   .WithMany(x => x.RoomImages)
                   .HasForeignKey(x => x.RoomId);
        }
    }
}
