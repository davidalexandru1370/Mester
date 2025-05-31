using Registry.Errors.Repositories;

namespace Registry.Errors.Services
{

    [Serializable]
    public class ServiceException : ApplicationException
    {
        public ServiceException() { }
        public ServiceException(string message) : base(message) { }
        public ServiceException(string message, Exception inner) : base(message, inner) { }

        public ServiceException(RepositoryException e) : base(e.Message, e) { }
    }
}
