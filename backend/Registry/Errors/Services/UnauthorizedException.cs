namespace Registry.Errors.Services
{

    [Serializable]
    public class UnauthorizedException : ServiceException
    {
        public UnauthorizedException() : base("Unauthorized") { }
    }
}
