namespace Registry.Models
{
    public class Bill
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required Job Job { get; set; }
        public required string Description { get; set; }
        public required string BillImage { get; set; }
        public decimal Amount { get; set; }
        public bool Paid { get; set; }
    }
}
