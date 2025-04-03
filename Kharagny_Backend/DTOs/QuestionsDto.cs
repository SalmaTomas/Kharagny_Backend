namespace Kharagny_Backend.DTOs
{
    public class QuestionsDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<ChoiceDto> Choices { get; set; }

    }
}
