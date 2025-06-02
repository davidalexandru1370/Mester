namespace Registry.Models
{
    public class TradesManJobResponse
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Conversation Conversation { get; set; } = default!;
        public required Guid ConversationId { get; set; }
        public required decimal WorkmanshipAmount { get; set; }
        public required DateTime AproximationEndDate { get; set; }
        public DateTime Sent { get; set; }
        public DateTime? Seen { get; set; }
    }
}
