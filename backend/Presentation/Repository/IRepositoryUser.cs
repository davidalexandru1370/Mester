using Registry.Models;

namespace Registry.Repository;

public interface IRepositoryUser
{
    Task Add(User user);

    Task<User?> FindByUsername(string username);
}
