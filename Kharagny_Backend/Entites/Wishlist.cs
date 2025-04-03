namespace Kharagny_Backend.Entites
{
    public class Wishlist
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int VenueId { get; set; }
        public User User { get; set; }
        public Venue Venue { get; set; }

    }
}
