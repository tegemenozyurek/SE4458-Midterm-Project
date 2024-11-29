using Microsoft.AspNetCore.Mvc;
using API_Project;
using System.ComponentModel;


namespace API_Project.Controllers
{
    [Route("api/v{version:apiVersion}/host")]
    [ApiController]
    [ApiVersion("1.0")]
    public class HostController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HostController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("listings")]
        [DisplayName("Insert Listing")]
        public IActionResult InsertListing([FromBody] Listing listing)
        {
            if (listing == null || string.IsNullOrWhiteSpace(listing.Country) || string.IsNullOrWhiteSpace(listing.City))
            {
                return BadRequest("Invalid listing details.");
            }

            _context.Listings.Add(listing);
            _context.SaveChanges();

            return Ok(new { Status = "Success", Message = "Listing added successfully." });
        }
    }
}
