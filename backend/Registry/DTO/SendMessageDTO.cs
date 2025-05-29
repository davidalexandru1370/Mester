namespace Registry.DTO
{
    public class SendMessageDTO
    {
        public required Guid ConversationId { get; set; }
        public required string Text { get; set; }
    }
}
