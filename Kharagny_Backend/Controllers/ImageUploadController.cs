using Kharagny_Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;

namespace Kharagny_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            string tempPath = Path.GetTempFileName();
            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var metadata = ImageMetadataService.ExtractImageMetadata(tempPath);


            string formattedDate = metadata.DateTaken != null ? FormatDate(metadata.DateTaken) : "Unknown Date";
            //string formattedDate = FormatDate(metadata.DateTaken);
            string formattedLatitude = metadata.Latitude.HasValue ? ConvertToDMS(metadata.Latitude.Value, "N", "S") : "Unknown Latitude";
            string formattedLogitude = metadata.Longitude.HasValue ? ConvertToDMS(metadata.Longitude.Value, "E", "W") : "Unknown Longitude";

            string placeName = "Unknown Location";
            if (metadata.Latitude.HasValue && metadata.Longitude.HasValue)
            {
                placeName = await GetLocationName(metadata.Latitude.Value, metadata.Longitude.Value);
            }

            return Ok(new
            {
                Message = "Metadata Extracted Successfully",
                DateTaken = formattedDate,
                Location = new
                {
                    Latitude = formattedLatitude,
                    Longitude = formattedLogitude,
                    PlaceName = placeName
                }
                //MetadataTags = metadata.Tags // Include all extracted metadata
            });
        }
        private string FormatDate(string date)
        {
            string[] formats = { "yyyy:MM:dd", "yyyy:MM:dd HH:mm:ss", "yyyy:MM:dd HH:mm:ss zzz" };
            if (DateTime.TryParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate.ToString("MMMM d, yyyy 'at' HH:mm 'UTC'");
            }
            return date;
        }

        private string ConvertToDMS(double decimalDegrees, string positive, string negative)
        {
            char direction = decimalDegrees >= 0 ? positive[0] : negative[0];
            decimalDegrees = Math.Abs(decimalDegrees);

            int degrees = (int)decimalDegrees;
            double minutesDecimal = (decimalDegrees - degrees) * 60;
            int minutes = (int)minutesDecimal;
            double seconds = (minutesDecimal - minutes) * 60;

            return $"{degrees}° {minutes}' {seconds:F2}\" {direction}";
        }

        private async Task<string> GetLocationName(double latitude, double longitude)
        {
            string url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={latitude}&lon={longitude}";


            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "KharagnyWebsite/1.0 (contact: salmatomas7599@gmail.com)");
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                    {
                        var root = doc.RootElement;
                        if (root.TryGetProperty("display_name", out JsonElement displayName))
                        {
                            return displayName.GetString();
                        }
                    }
                }

            }
            return "Unknown Location";
        }
    }
}
