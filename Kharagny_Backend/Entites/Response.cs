namespace Kharagny_Backend.Entites
{
    public class Response
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public int QuestionId { get; set; }
        public string SelectedChoices { get; set; }

    }
}
