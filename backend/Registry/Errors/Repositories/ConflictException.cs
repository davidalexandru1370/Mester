namespace Registry.Errors.Repositories
{
    [Serializable]
    public class ConflictException : RepositoryException
    {
        public ConflictException() { }
        public ConflictException(string message) : base(message) { }
        public ConflictException(string message, Exception inner) : base(message, inner) { }
    }
}
