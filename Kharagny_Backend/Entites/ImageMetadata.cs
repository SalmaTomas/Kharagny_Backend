namespace Kharagny_Backend.Entites
{
    public class ImageMetadata
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string DateTaken { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Address { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    }
}
