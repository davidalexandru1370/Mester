namespace Registry.Models
{
    public class Conversation
    {
        public Guid Id { get; set; }
        // The id of the first user should be smaller than the id of the second user
        public required User User1 { get; set; }
        public required User User2 { get; set; }
        public List<Message> Messages { get; set; } = new();
    }
}
