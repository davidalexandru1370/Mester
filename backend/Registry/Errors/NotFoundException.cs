namespace Registry.Errors
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException() : base("Not found") { }
        public NotFoundException(string message) : base(message) { }
    }

}
