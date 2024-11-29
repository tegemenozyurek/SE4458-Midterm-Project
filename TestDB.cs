using Microsoft.AspNetCore.Mvc;
using API_Project;

namespace API_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("listings")]
        public IActionResult GetListings()
        {
            var listings = _context.Listings.ToList();
            return Ok(listings);
        }
    }
}
