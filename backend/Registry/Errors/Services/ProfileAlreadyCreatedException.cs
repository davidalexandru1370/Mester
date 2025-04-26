namespace Registry.Errors.Services
{
    public class ProfileAlreadyCreatedException : ServiceException
    {
        public ProfileAlreadyCreatedException() : base("Trades man profile already created") { }
    }
}
