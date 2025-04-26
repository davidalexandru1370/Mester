using Microsoft.EntityFrameworkCore;
using Registry.DTO;
using Registry.Errors.Services;
using Registry.Models;
using Registry.Repository;

namespace Registry.Services
{
    public class TradesManService
    {
        // A bit lazy to create repo for everything...should we use just the context?
        private readonly TradesManDbContext _context;

        public TradesManService(TradesManDbContext context)
        {
            _context = context;
        }


        public async Task<List<string>> GetSpecialities()
        {
            return await _context.Specialties.Select(s => s.Type).ToListAsync();
        }

        public async Task<Specialty?> FindSpeciality(string Type)
        {
            return await _context.Specialties.FirstOrDefaultAsync(x => x.Type == Type);
        }

        public async Task<List<Specialty>> GetSpecialitiesByName(IList<string> specialitiesTypeNames)
        {
            var specialities = new List<Specialty>();
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

            user.TradesManProfile = new TradesMan { Specialties = specialities };
            _context.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
