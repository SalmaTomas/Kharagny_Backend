
using MetadataExtractor;

namespace Kharagny_Backend.Services
{
    public class ImageMetadataService
    {
        public static (string DateTaken, double? Latitude, double? Longitude, Dictionary<string, string> Tags) ExtractImageMetadata(string imagePath)
        {
            string dateTaken = null;
            double? latitude = null;
            double? longitude = null;
            var metadataTags = new Dictionary<string, string>();

            try
            {
                var directories = ImageMetadataReader.ReadMetadata(imagePath);


                foreach (var directory in directories)
                {
                    foreach (var tag in directory.Tags)
                    {
                        metadataTags[tag.Name] = tag.Description;
                        if (tag.Name.Contains("Date Created", StringComparison.OrdinalIgnoreCase) ||
                            tag.Name.Contains("Original", StringComparison.OrdinalIgnoreCase))
                        {
                            dateTaken = tag.Description;
                        }

                        if (tag.Name == "GPS Latitude" && metadataTags.ContainsKey("GPS Latitude Ref"))
                        {
                            string direction = metadataTags["GPS Latitude Ref"];

                            latitude = ConvertGpsCoordinate(tag.Description + " " + direction);
                        }

                        if (tag.Name == "GPS Longitude" && metadataTags.ContainsKey("GPS Longitude Ref"))
                        {
                            string direction = metadataTags["GPS Longitude Ref"];

                            longitude = ConvertGpsCoordinate(tag.Description + " " + direction);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                metadataTags["Error"] = "Error reading image metadata: " + ex.Message;
            }
            return (dateTaken, latitude, longitude, metadataTags);
        }
        private static double ConvertGpsCoordinate(string coordinate)
        {
            string[] parts = coordinate.Split(new[] { '°', '\'', '"' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 3)
            {
                throw new FormatException("Invalid GPS coordinate format");
            }

            double degrees = double.Parse(parts[0].Trim());
            double minutes = double.Parse(parts[1].Trim());
            double seconds = double.Parse(parts[2].Trim());
            //string direction = parts[3];

            double decimalCoordinate = degrees + (minutes / 60) + (seconds / 3600);

            if (coordinate.Contains("S") || coordinate.Contains("W"))
            {
                decimalCoordinate *= -1;
            }
            return decimalCoordinate;
        }
    }
}
