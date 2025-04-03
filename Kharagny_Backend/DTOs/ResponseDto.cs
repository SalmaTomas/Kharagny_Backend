namespace Kharagny_Backend.DTOs
{
    public class ResponseDto
    {
        public int QuestionId { get; set; }
        public List<int> SelectedChoiceIds { get; set; } // List of selected choices

    }
}
