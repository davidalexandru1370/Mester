namespace Registry.Models;

public class TradesMan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required List<Specialty> Specialties { get; set; }
    public required string Description { get; set; }

}
