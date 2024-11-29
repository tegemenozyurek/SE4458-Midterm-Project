using Microsoft.EntityFrameworkCore;

namespace API_Project
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Listing> Listings { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
    }

    public class Listing
    {
        public int ListingID { get; set; }
        public int NoOfPeople { get; set; }
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public float? Rating { get; set; }
    }

    public class Booking
    {
        public int BookingID { get; set; }
        public int ListingID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NoOfPeople { get; set; }
    }

    public class Review
    {
        public int ReviewID { get; set; }
        public int BookingID { get; set; }
        public float Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
