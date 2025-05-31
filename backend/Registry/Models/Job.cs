namespace Registry.Models;

public class Job
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required ClientJobRequest JobRequest { get; set; }
    public required TradesManJobResponse TradesManJobResponse { get; set; }

    public required DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
