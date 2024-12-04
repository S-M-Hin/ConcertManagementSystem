

using CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection.Emit;
using System;

namespace CMS.Website.Data
{
    public class ConcertDbContext : DbContext
    {
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Concert> Concerts { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        public ConcertDbContext(DbContextOptions<ConcertDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Concert>()
                .Property(c => c.TicketPrice)
                .HasColumnType("decimal(18,2)");


            modelBuilder.Entity<Ticket>()
                .Property(t => t.Price)
                .HasColumnType("decimal(18,2)");

        }
    }
}
