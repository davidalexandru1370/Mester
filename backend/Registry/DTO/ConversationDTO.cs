namespace Registry.DTO
{
    public class ConversationDTO
    {
        public required Guid Id { get; set; }
        public required ConversationUserDTO With { get; set; }
        public MessageDTO? LastMessage { get; set; }
    }
}
