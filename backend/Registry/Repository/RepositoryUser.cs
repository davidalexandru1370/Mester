using Microsoft.EntityFrameworkCore;
using Registry.Errors;
using Registry.Errors.Repositories;
using Registry.Models;

namespace Registry.Repository
{
    public class RepositoryUser : IRepositoryUser
    {
        private readonly TradesManDbContext _context;

        public RepositoryUser(TradesManDbContext context)
        {
            _context = context;
        }

        public async Task<User> Add(User user)
        {
            try
            {
                var createdUser = await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return createdUser.Entity;
            }
            catch (DbUpdateException e)
            {
                throw RepositoryException.From(e);
            }
        }

        public async Task Modify(User user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw RepositoryException.From(e);
            }
        }

        public async Task<User?> FindByUsername(string email)
        {
            var user = await _context.Users
                .Where(x => x.Name == email)
                .Include(x => x.TradesManProfile)
                .FirstOrDefaultAsync();
            return user;
        }

        public async Task<User> GetUserById(Guid userId)
        {
            return await _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.TradesManProfile)
                .FirstOrDefaultAsync() ?? throw new NotFoundException();
        }

        public async Task UpdateUserImage(Guid userId, string imageUrl)
        {
            var user = await _context.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();

            if (user is null)
            {
                throw new NotFoundException();
            }

            user.ImageUrl = imageUrl;
            _context.Users.Update(user);

            await _context.SaveChangesAsync();
        }
    }
}
