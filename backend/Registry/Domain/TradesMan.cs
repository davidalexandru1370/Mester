namespace Registry.Domain
{
    public class TradesMan
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }

        public required List<Specialty> Specialties { get; set; }
    }
}
