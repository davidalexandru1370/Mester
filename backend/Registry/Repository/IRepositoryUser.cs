using Registry.Models;

namespace Registry.Repository;

public interface IRepositoryUser
{
    Task<User> Add(User user);
    Task Modify(User user);
    Task<User?> FindByUsername(string username);
    Task <User> GetUserById(Guid userId);
    Task UpdateUserImage(Guid userId, string  imageUrl);
}
