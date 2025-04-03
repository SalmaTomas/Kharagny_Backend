namespace Kharagny_Backend.Entites
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public ICollection<Choice> Choices { get; set; }

    }
}
