namespace Registry.DTO.Responses
{
    public class BillDTO
    {
        public required Guid Id { get; set; }
        //TODO: what do we need here instead of the job?
        public required Guid JobId { get; set; }
        public required string Description { get; set; }
        public required string BillImage { get; set; }
        public required decimal Amount { get; set; }
        public required bool Paid { get; set; }
    }
}
