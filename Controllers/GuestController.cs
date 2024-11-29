using Microsoft.AspNetCore.Mvc;
using API_Project;
using System;
using System.Linq;
using System.ComponentModel;

namespace API_Project.Controllers
{
    [Route("api/v{version:apiVersion}/guest")]
    [ApiController]
    [ApiVersion("1.0")]
    public class GuestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GuestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("listings")]
        [DisplayName("Query Listings")]
        public IActionResult QueryListings(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int? noOfPeople,
            [FromQuery] string country,
            [FromQuery] string city,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.Listings.AsQueryable();

            if (!string.IsNullOrWhiteSpace(country))
                query = query.Where(l => l.Country.ToLower() == country.ToLower());

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(l => l.City.ToLower() == city.ToLower());

            if (noOfPeople.HasValue)
                query = query.Where(l => l.NoOfPeople >= noOfPeople.Value);

            if (startDate.HasValue && endDate.HasValue)
            {
                var bookedListings = _context.Bookings
                    .Where(b => b.StartDate < endDate.Value && b.EndDate > startDate.Value)
                    .Select(b => b.ListingID)
                    .Distinct()
                    .ToList();

                query = query.Where(l => !bookedListings.Contains(l.ListingID));
            }

            var totalRecords = query.Count();
            var listings = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new
                {
                    l.ListingID,
                    l.Country,
                    l.City,
                    l.NoOfPeople,
                    l.Price,
                    l.Rating
                })
                .ToList();

            var response = new
            {
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                PageNumber = pageNumber,
                PageSize = pageSize,
                Listings = listings
            };

            return Ok(response);
        }

        [HttpPost("book")]
        [DisplayName("Book a Stay")]
        public IActionResult BookStay([FromBody] Booking booking)
        {
            if (booking == null || booking.StartDate >= booking.EndDate)
            {
                return BadRequest("Invalid booking dates.");
            }

            var existingBookings = _context.Bookings
                .Where(b => b.ListingID == booking.ListingID && b.StartDate < booking.EndDate && b.EndDate > booking.StartDate)
                .Any();

            if (existingBookings)
            {
                return BadRequest("Listing is already booked for the selected dates.");
            }

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return Ok(new { Status = "Success", Message = "Booking confirmed." });
        }

        [HttpPost("review")]
        [DisplayName("Review a Stay")]
        public IActionResult ReviewStay([FromBody] Review review)
        {
            if (review == null || review.Rating < 1 || review.Rating > 5)
            {
                return BadRequest("Invalid review data.");
            }

            var booking = _context.Bookings.FirstOrDefault(b => b.BookingID == review.BookingID);

            if (booking == null)
            {
                return BadRequest("Booking not found.");
            }

            _context.Reviews.Add(review);
            _context.SaveChanges();

            return Ok(new { Status = "Success", Message = "Review added successfully." });
        }
    }
}
