namespace Registry.DTO
{
    public class MessageDTO
    {
        public required Guid Id { get; set; }
        public required ConversationUserDTO From { get; set; }
        public required bool IsMe { get; set; }
        public required string Text { get; set; }
    }
}
