using Microsoft.EntityFrameworkCore;
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

        public async Task Add(User user)
        {
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
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

        public async Task<User?> FindByUsername(string username)
        {
            var user = await _context.Users
                .Where(x => x.Name == username)
                .Include(x => x.TradesManProfile)
                .FirstOrDefaultAsync();
            return user;
        }
    }
}
