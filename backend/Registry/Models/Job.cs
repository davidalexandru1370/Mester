namespace Registry.Models;

public class Job
{
    public required Guid Id { get; set; }
    public required User Client { get; set; }
    public required User TradesMan { get; set; }

    public required DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public required List<Speciality> JobTypes { get; set; }

}
