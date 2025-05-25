using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Registry.DTO;
using Registry.Errors.Services;
using Registry.Models;
using Registry.Repository;
using Registry.Services.Interfaces;

namespace Registry.Services
{
    // TODO: this should be in DTO or contracts?
    public record FilterListTradesMen(List<string>? Specialties);

    public class TradesManService : ITradesManService
    {
        // A bit lazy to create repo for everything...should we use just the context?
        private readonly TradesManDbContext _context;

        public TradesManService(TradesManDbContext context)
        {
            _context = context;
        }

        public async Task<Speciality> AddSpecialty(Speciality speciality)
        {
            try
            {
                await _context.Specialties.AddAsync(speciality);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return speciality;
        }

        public async Task<List<Speciality>> AddSpecialitiesBulk(List<Speciality> specialties)
        {
            try
            {
                await _context.Specialties.AddRangeAsync(specialties);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return specialties;
        }

        public async Task<List<string>> GetSpecialities()
        {
            return await _context.Specialties.Select(s => s.Type).ToListAsync();
        }

        public async Task<Speciality?> FindSpeciality(string Type)
        {
            return await _context.Specialties.FirstOrDefaultAsync(x => x.Type == Type);
        }

        public async Task<List<Speciality>> GetSpecialitiesByName(IList<string> specialitiesTypeNames)
        {
            var specialities = new List<Speciality>();
            var invalidSpecialities = new List<string>();
            foreach (var specialityName in specialitiesTypeNames)
            {
                var speciality = await FindSpeciality(specialityName);
                if (speciality is null)
                {
                    invalidSpecialities.Add(specialityName);
                }
                else
                {
                    specialities.Add(speciality);
                }
            }
            if (invalidSpecialities.Count > 0)
            {
                throw new InvalidSpecialitiesTypeException(invalidSpecialities);
            }
            return specialities;
        }

        public async Task UpdateTradesManProfile(User user, TradesManDTO tradesManDTO)
        {
            var specialities = await GetSpecialitiesByName(tradesManDTO.Specialties);

            //user.TradesManProfile = new TradesMan { Specialties = specialities, Description = tradesManDTO.Description };
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TradesManListDTO>> GetTradesManList(FilterListTradesMen filter)
        {
            //TODO: add sorting based on rating
            var query = _context.Users.Include(x => x.TradesManProfile)
                .ThenInclude(x => x.Specialities)
                .Where(x => x.TradesManProfile != null);

            if (filter.Specialties is not null)
            {
                query = query.Where(x => x.TradesManProfile!.Specialities.Select(x => x.Speciality)
                    .Any(x => filter.Specialties.Any(y => x.Type == y)));
            }

            return await query
                .Select(x => new TradesManListDTO
                {
                    Id = x.Id,
                    Description = x.TradesManProfile!.Description,
                    Name = x.Name,
                    Specialities = x.TradesManProfile!.Specialities.Select(x => x.Speciality).ToList()
                })
                .ToListAsync();
        }

        public async Task<TradesManInfoPageDTO?> GetTradesManInfo(Guid id)
        {
            var r = await _context.Users.Include(x => x.TradesManProfile)
                .ThenInclude(x => x.Specialities)
                .Where(x => x.TradesManProfile != null)
                .FirstAsync(x => x.Id == id);

            if (r is null) return null;

            return new TradesManInfoPageDTO
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.TradesManProfile!.Description,
                Specialities = r.TradesManProfile.Specialities.Select(x => x.Speciality).ToList()
            };
        }
    }
}
