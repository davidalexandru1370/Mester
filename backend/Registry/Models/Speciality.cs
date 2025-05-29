namespace Registry.Models;

public class Speciality
{
    public required Guid Id { get; set; }
    public required string Type { get; set; }
    public required string ImageUrl { get; set; }
    public required List<TradesManSpecialities> TradesMenSpecialities { get; set; }
}
