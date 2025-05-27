namespace Registry.Models;

public class Speciality
{
    public required Guid Id { get; set; }
    public required string Type { get; set; }
    public string ImageUrl { get; set; }
    public List<TradesManSpecialities> TradesMenSpecialities { get; set; }
}
