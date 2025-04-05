using Microsoft.VisualBasic;

namespace Registry.Domain
{
    public class Job
    {
        public required Guid Id { get; set; }
        public required Client Client { get; set; }
        public required TradesMan TradesMan { get; set; }

        public required DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<Specialty> JobTypes { get; set; }

    }
}
