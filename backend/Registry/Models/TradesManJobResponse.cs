namespace Registry.Models
{
    public class TradesManJobResponse
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public ClientJobRequest ClientJobRequest { get; set; } = default!;
        public required Guid ClientJobRequestId { get; set; }
        public User TradesMan { get; set; } = default!;
        public required Guid TradesManId { get; set; }
        public required decimal WorkmanshipAmount { get; set; }
        public required DateTime AproximationEndDate { get; set; }
        public DateTime Sent { get; set; }
        public DateTime? Seen { get; set; }
    }
}
