using Registry.Models;

namespace Registry.DTO
{
    public class JobDTO
    {
        public required Guid Id { get; set; }
        public required ClientJobRequest JobRequest { get; set; }
        public required User TradesMan { get; set; }

        public required DateTime StartDate { get; set; }
        public required DateTime? EndDate { get; set; }
    }
}
