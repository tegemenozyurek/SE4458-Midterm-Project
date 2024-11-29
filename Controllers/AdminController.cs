using Microsoft.AspNetCore.Mvc;
using API_Project;
using System.Linq;
using System.ComponentModel;

namespace API_Project.Controllers
{
    [Route("api/v{version:apiVersion}/admin")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("report")]
        [DisplayName("Report Listings")]
        public IActionResult ReportListings(
            [FromQuery] string country,
            [FromQuery] string? city,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.Listings.AsQueryable();

            if (!string.IsNullOrWhiteSpace(country))
                query = query.Where(l => l.Country.ToLower() == country.ToLower());

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(l => l.City.ToLower() == city.ToLower());

            var totalRecords = query.Count();
            var listings = query
                .OrderByDescending(l => l.Rating)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new
            {
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                PageNumber = pageNumber,
                PageSize = pageSize,
                Listings = listings
            };

            if (!listings.Any())
            {
                return NotFound(new { Message = "No listings found." });
            }

            return Ok(response);
        }
    }
}
