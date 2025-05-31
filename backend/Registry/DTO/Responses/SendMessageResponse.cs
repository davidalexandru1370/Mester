namespace Registry.DTO.Responses
{
    public class SendMessageResponse
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Text { get; set; }
        public DateTime Sent { get; set; }
        public required ConversationUserDTO From { get; set; }
    }
}
