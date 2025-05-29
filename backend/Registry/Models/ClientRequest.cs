namespace Registry.Models
{
    public class ClientRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required User From { get; set; }
        public required User To { get; set; }

        public DateTime RequestedOn { get; set; }
        public required string Description { get; set; }

        public decimal? WorkmanshipAmount { get; set; }

        public DateTime? AproximateEndDate { get; set; }

        public Job? Approved { get; set; }

    }
}
