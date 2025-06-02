namespace Registry.DTO.Responses
{
    public class TradesManJobResponseDTO
    {
        public required Guid Id { get; set; }
        public required DateTime Sent { get; set; }
        public required DateTime? Seen { get; set; }
        public required Guid ConversationId { get; set; }
        public required decimal WorkmanshipAmount { get; set; }
        public required DateTime AproximationEndDate { get; set; }

    }
}
