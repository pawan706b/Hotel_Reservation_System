using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Hotel_Reservation_System.Models;

namespace Hotel_Reservation_System.Data
{
    public class Hotel_Reservation_SystemContext : DbContext
    {
        public Hotel_Reservation_SystemContext (DbContextOptions<Hotel_Reservation_SystemContext> options)
            : base(options)
        {
        }

        public DbSet<Hotel_Reservation_System.Models.User> User { get; set; } = default!;

        public DbSet<Hotel_Reservation_System.Models.Manager> Manager { get; set; } = default!;

        public DbSet<Hotel_Reservation_System.Models.Hotel> Hotel { get; set; } = default!;

        public DbSet<Hotel_Reservation_System.Models.Room> Room { get; set; } = default!;

        public DbSet<Hotel_Reservation_System.Models.Reservation> Reservation { get; set; } = default!;

        public DbSet<Hotel_Reservation_System.Models.Billing> Billing { get; set; } = default!;
    }
}
