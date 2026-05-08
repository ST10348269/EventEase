using Microsoft.EntityFrameworkCore;
using EventEase.Models;

namespace EventEase.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingDetailsView> BookingDetailsViews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasOne(b => b.Event)
                    .WithMany()
                    .HasForeignKey(b => b.EventID)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(b => b.Venue)
                    .WithMany()
                    .HasForeignKey(b => b.VenueID)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Map BookingDetailsView to the SQL view (no primary key needed - keyless)
            modelBuilder.Entity<BookingDetailsView>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("vw_BookingDetails");
            });
        }
    }
}
