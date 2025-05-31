namespace Registry.Errors
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException() : base("Not found") { }
    }

}
