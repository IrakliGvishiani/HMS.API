using HMS.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace HMS.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            base.OnModelCreating(builder);

            builder.RenameIdentityTables();

        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Guest> Guests { get; set; }

        public DbSet<Hotel> Hotels { get; set; }

        public DbSet<Manager> Managers { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<ReservationRoom> ReservationRooms { get; set; }

        public DbSet<Room> Rooms { get; set; }

        public DbSet<Admin> Admins { get; set; }
    }
}
