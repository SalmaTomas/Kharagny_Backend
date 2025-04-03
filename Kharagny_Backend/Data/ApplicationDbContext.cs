using Kharagny_Backend.Entites;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kharagny_Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<ImageMetadata> Images { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany(u => u.wishlist)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.Venue)
                .WithMany(v => v.wishlist)
                .HasForeignKey(w => w.VenueId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
