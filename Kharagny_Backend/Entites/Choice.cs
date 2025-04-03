using System.Text.Json.Serialization;

namespace Kharagny_Backend.Entites
{
    public class Choice
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int QuestionId { get; set; }
        [JsonIgnore]
        public Question Question { get; set; }
    }
}
