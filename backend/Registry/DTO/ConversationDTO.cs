namespace Registry.DTO
{
    public class ConversationDTO
    {
        public required Guid Id { get; set; }
        public required ConversationUserDTO With { get; set; }
        public required MessageDTO LastMessage { get; set; }
    }
}
