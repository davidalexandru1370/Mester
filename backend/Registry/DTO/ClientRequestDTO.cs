namespace Registry.DTO
{
    public class ClientRequestDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required ConversationUserDTO With { get; set; }
        public DateTime RequestedOn { get; set; }
        public required string Description { get; set; }

        public decimal? WorkmanshipAmount { get; set; }

        public DateTime? AproximateEndDate { get; set; }

        // TODO: maybe this is nod needed
        public JobDTO? Approved { get; set; }
    }
}
