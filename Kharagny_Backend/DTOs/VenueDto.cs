namespace Kharagny_Backend.DTOs
{
    public class VenueDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }

        public double Rating { get; set; }
        public string Description { get; set; }
        public string OpeningHours { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public double Discount { get; set; }
    }
}
