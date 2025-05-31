namespace Registry.Errors.Repositories
{
    [Serializable]
    public class RepoConflictException : RepositoryException
    {
        public RepoConflictException() { }
        public RepoConflictException(string message) : base(message) { }
        public RepoConflictException(string message, Exception inner) : base(message, inner) { }
    }
}
