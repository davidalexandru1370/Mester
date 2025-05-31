namespace Registry.Errors.Services
{

    [Serializable]
    public class NameAlreadyUsedException : ConflictException
    {
        public NameAlreadyUsedException() : base("Username already used") { }
        public NameAlreadyUsedException(string message) : base(message) { }
        public NameAlreadyUsedException(Exception inner) : base("Username already used", inner) { }
    }
}
