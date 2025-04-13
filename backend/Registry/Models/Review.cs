namespace Registry.Models;

public class Review
{
    public Guid Id{ get; set; }
    public required double Rating{ get; set; }
    public required string Comment { get; set; }

    public required Job Job { get; set; }
}
