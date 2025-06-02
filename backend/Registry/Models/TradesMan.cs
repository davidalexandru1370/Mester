namespace Registry.Models;

public class TradesMan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Description { get; set; }
    public string City { get; set; }
    public string County { get; set; }
    public List<TradesManSpecialities> Specialities { get; set; }
    public List <TradesManImages> Images { get; set; }
}
