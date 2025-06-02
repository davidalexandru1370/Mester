namespace Registry.Models;


// There is something weird going on between Response, Conversation, Request and Job.
// I don't think this table is normalized
public class Job
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public TradesManJobResponse TradesManJobResponse { get; set; } = default!;
    public required Guid TradesManJobResponseId { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime? EndDate { get; set; }

    public List<Bill> Bills { get; set; } = new();
}
