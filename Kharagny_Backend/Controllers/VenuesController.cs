using Kharagny_Backend.Data;
using Kharagny_Backend.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kharagny_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VenuesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VenuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get venues for home page (pagination)
        [HttpGet("home")]
        public async Task<ActionResult<IEnumerable<VenueCardDto>>> GetHomeVenues(int page = 1, int pageSize = 10)
        {
            var venues = await _context.Venues
                .OrderBy(v => v.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(v => new VenueCardDto
                {
                    Id = v.Id,
                    Name = v.Name,
                    Category = v.Category,
                    ImageUrl = v.ImageUrl,
                    Discount = v.Discount
                })
                .ToListAsync();

            return Ok(venues);
        }

        // Get venues by category
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<VenueDto>>> GetVenuesByCategory(string category)
        {
            var venues = await _context.Venues
                .Where(v => v.Category.ToLower() == category.ToLower())
                .Select(v => new VenueDto
                {
                    Id = v.Id,
                    Name = v.Name,
                    Address = v.Address,
                    Category = v.Category,
                    SubCategory = v.SubCategory, 
                    Rating = v.Rating,
                    Description = v.Description,
                    OpeningHours = v.OpeningHours,
                    ImageUrl = v.ImageUrl,
                    Price = v.Price,
                    Discount = v.Discount
                })
                .ToListAsync();

            return Ok(venues);
        }

        // Get venues by category and subcategory
        [HttpGet("category/{category}/subcategory/{subcategory}")]
        public async Task<ActionResult<IEnumerable<VenueDto>>> GetVenuesByCategoryAndSubCategory(string category, string subcategory)
        {
            var venues = await _context.Venues
                .Where(v => v.Category.ToLower() == category.ToLower() && v.SubCategory.ToLower() == subcategory.ToLower())
                .Select(v => new VenueDto
                {
                    Id = v.Id,
                    Name = v.Name,
                    Address = v.Address,
                    Category = v.Category,
                    SubCategory = v.SubCategory, 
                    Rating = v.Rating,
                    Description = v.Description,
                    OpeningHours = v.OpeningHours,
                    ImageUrl = v.ImageUrl,
                    Price = v.Price,
                    Discount = v.Discount
                })
                .ToListAsync();

            return Ok(venues);
        }

        // detailed information for a single venue
        [HttpGet("{id}")]
        public async Task<ActionResult<VenueDto>> GetVenueDetails(int id)
        {
            var venue = await _context.Venues
                .Where(v => v.Id == id)
                .Select(v => new VenueDto
                {
                    Id = v.Id,
                    Name = v.Name,
                    Address = v.Address,
                    Category = v.Category,
                    SubCategory = v.SubCategory, 
                    Rating = v.Rating,
                    Description = v.Description,
                    OpeningHours = v.OpeningHours,
                    ImageUrl = v.ImageUrl,
                    Price = v.Price,
                    Discount = v.Discount
                })
                .FirstOrDefaultAsync();

            if (venue == null)
                return NotFound("Venue not found.");

            return Ok(venue);
        }
    }
}
