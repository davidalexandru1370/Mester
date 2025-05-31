using Registry.Models;

namespace Registry.Repository;

public interface IRepositoryUser
{
    Task<User> Add(User user);
    Task Modify(User user);
    Task<User?> FindByEmail(string email);
    Task <User> GetUserById(Guid userId);
    Task UpdateUserImage(Guid userId, string  imageUrl);
}
