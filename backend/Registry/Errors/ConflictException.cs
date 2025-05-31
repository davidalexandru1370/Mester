namespace Registry.Errors
{

    [Serializable]
    public class ConflictException : ApplicationException
    {
        public ConflictException() : base("Conflict") { }
        public ConflictException(string message) : base(message) { }
        public ConflictException(string message, Exception inner) : base(message, inner) { }
    }

}
