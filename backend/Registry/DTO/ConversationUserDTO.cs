namespace Registry.DTO
{
    public class ConversationUserDTO
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public string? ImageUrl { get; set; }
    }
}
