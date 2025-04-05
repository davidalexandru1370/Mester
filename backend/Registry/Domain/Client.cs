namespace Registry.Domain
{
    public class Client
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }

        public required string PhoneNumber { get; set; }

    }
}
