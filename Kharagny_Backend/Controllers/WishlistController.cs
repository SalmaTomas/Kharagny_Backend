using Kharagny_Backend.Data;
using Kharagny_Backend.DTOs;
using Kharagny_Backend.Entites;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kharagny_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        public WishlistController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Add new Venue to Wishlist
        [HttpPost("add")]
        public async Task<IActionResult> AddToWishlist([FromQuery] string userId, [FromQuery] int venueId)
        {
            var exists = await _context.Wishlists.AnyAsync(w => w.UserId == userId && w.VenueId == venueId);
            if (exists)
            {
                return BadRequest("Venue is already in your wishlist.");
            }

            var wishlistItem = new Wishlist { UserId = userId, VenueId = venueId };
            _context.Wishlists.Add(wishlistItem);
            await _context.SaveChangesAsync();
            return Ok("Venue added to wishlist");
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromWishlist([FromQuery] string userId, [FromQuery] int venueId)
        {
            var wishlistItem = await _context.Wishlists.FirstOrDefaultAsync(w => w.UserId == userId && w.VenueId == venueId);
            if (wishlistItem == null)
            {
                return NotFound("Venue not found in wishlist");
            }

            _context.Wishlists.Remove(wishlistItem);
            await _context.SaveChangesAsync();
            return Ok("Venue removed from wishlist");
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetWishlist([FromQuery] string userId)
        {
            var wishlist = await _context.Wishlists
                .Where(w => w.UserId == userId)
                .Select(w => new VenuesWishlistDto
                {
                    Id = w.Venue.Id,
                    Name = w.Venue.Name,
                    Address = w.Venue.Address,
                    Category = w.Venue.Category,
                    Subcategory = w.Venue.SubCategory,
                    Rating = w.Venue.Rating,
                    Description = w.Venue.Description,
                    OpeningHours = w.Venue.OpeningHours,
                    ImageUrl = w.Venue.ImageUrl,
                    Price = w.Venue.Price,
                    Discount = (double)w.Venue.Discount
                })
                .ToListAsync();
            return Ok(wishlist);
        }

    }
}
