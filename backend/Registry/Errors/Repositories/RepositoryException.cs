using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Registry.Errors.Repositories
{

    [Serializable]
    public class RepositoryException : ApplicationException
    {
        public RepositoryException() { }
        public RepositoryException(string message) : base(message) { }
        public RepositoryException(string message, Exception inner) : base(message, inner) { }


        public static RepositoryException From(DbUpdateException exception)
        {
            if (exception.InnerException is not SqlException sqlEx)
            {
                return new RepositoryException(exception.Message, exception);
            }
            if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
            {
                return new RepoConflictException("Key already used", sqlEx);
            }
            return new RepositoryException(exception.Message, exception);
        }
    }
}
