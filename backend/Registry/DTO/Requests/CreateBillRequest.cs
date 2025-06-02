namespace Registry.DTO.Requests
{
    public class CreateBillRequest
    {
        public required string Description { get; set; }
        public required string BillImageBase64 { get; set; }
        public required decimal Amount { get; set; }
    }
}
