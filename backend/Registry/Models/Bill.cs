namespace Registry.Models
{
    public class Bill
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Job Job { get; set; } = default!;
        public required Guid JobId { get; set; }
        public required string Description { get; set; }
        public required string BillImage { get; set; }
        public required decimal Amount { get; set; }
        public bool Paid { get; set; } = false;
        public DateTime? Seen { get; set; }
        public DateTime Sent { get; set; } = DateTime.Now;
    }
}
