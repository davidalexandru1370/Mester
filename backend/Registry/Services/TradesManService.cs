using Microsoft.EntityFrameworkCore;
using Registry.DTO;
using Registry.Errors.Services;
using Registry.Models;
using Registry.Repository;
using Registry.Services.Interfaces;

namespace Registry.Services
{
    public record FilterListTradesMen(List<string>? Specialties);

    public class TradesManService : ITradesManService
    {
        private readonly TradesManDbContext _context;
        private readonly IImageService _imageService;

        public TradesManService(TradesManDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
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
            var specialities = (await GetSpecialitiesByName(tradesManDTO.Specialities.Select(s => s.Name).ToList())).Select((s, index) => new TradesManSpecialities
            {
                Price = tradesManDTO.Specialities[index].Price,
                SpecialityId = s.Id,
                TradesManId = user.Id,
                UnitOfMeasure = tradesManDTO.Specialities[index].UnitOfMeasure
            }).ToList();

            await _context.TradesManSpecialities.AddRangeAsync(specialities);

            user.TradesManProfile = new TradesMan
            {
                Specialities = specialities,
                Description = tradesManDTO.Description,
                City = tradesManDTO.City,
                County = tradesManDTO.County
            };

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
                    City = x.TradesManProfile.City,
                    County = x.TradesManProfile.County,
                    Specialities = x.TradesManProfile!.Specialities.Select(x => new SpecialityDTO
                    {
                        SpecialityId = x.SpecialityId,
                        TradesManId = x.TradesManId,
                        Price = x.Price,
                        Type = x.Speciality.Type,
                        UnitOfMeasure = x.UnitOfMeasure,
                        ImageUrl = x.Speciality.ImageUrl,
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<TradesManInfoPageDTO?> GetTradesManInfo(Guid id)
        {
            var r = await _context.Users
                .Include(x => x.TradesManProfile)
                    .ThenInclude(x => x.Images)
                .Include(x => x.TradesManProfile)
                    .ThenInclude(x => x.Specialities)
                    .ThenInclude(x => x.Speciality)
                .Where(x => x.TradesManProfile != null)
                .FirstOrDefaultAsync(x => x.Id == id); // Changed FirstAsync to FirstOrDefaultAsync to handle null case

            if (r is null)
            {
                return null;
            }

            return new TradesManInfoPageDTO
            {
                Id = r.Id,
                ImageUrl = r.ImageUrl,
                Name = r.Name,
                City = r.TradesManProfile!.City,
                County = r.TradesManProfile!.County,
                Description = r.TradesManProfile!.Description,
                Images = r.TradesManProfile.Images.Select(i => new ImageDTO
                {
                    ImageUrl = i.ImageUrl
                }).ToList() ?? new List<ImageDTO>(),
                Specialities = r.TradesManProfile.Specialities.Select(x => new SpecialityDTO
                {
                    SpecialityId = x.SpecialityId,
                    TradesManId = x.TradesManId,
                    Price = x.Price,
                    Type = x.Speciality.Type,
                    UnitOfMeasure = x.UnitOfMeasure,
                    ImageUrl = x.Speciality.ImageUrl,
                }).ToList() ?? new List<SpecialityDTO>(),
            };
        }

        public async Task<List<FindTradesManDTO>> FindTradesMen(string pattern, int limit)
        {
            pattern = pattern.Replace("%", "\\%");
            pattern = pattern.Replace("_", "\\_");
            var tradesMen = _context.Users
                .Include(u => u.TradesManProfile)
                .Where(u => u.TradesManProfile != null)
                .Where(u => EF.Functions.Like(u.Name, $"%{pattern}%"))
                // TODO: order the profiles based on the rating
                .OrderBy(u => u.Name)
                .Take(limit)
                .AsAsyncEnumerable();
            var r = new List<FindTradesManDTO>();
            await foreach (var user in tradesMen)
            {
                r.Add(user.ToFindTradesManDTO());
            }
            return r;
        }

        public async Task AddWorkorderImages(Guid tradesManId, List<IFormFile> images)
        {
            var tradesmanImages = new List<TradesManImages>();

            foreach (var image in images)
            {
                var imageUrl = await _imageService.UploadImage(image);
                tradesmanImages.Add(new TradesManImages
                {
                    ImageUrl = imageUrl,
                    TradesmanId = tradesManId,
                });
            }

            _context.TradesManImages.AddRange(tradesmanImages);
            await _context.SaveChangesAsync();
        }

        public async Task AddWorkorderImages(Guid tradesManId, List<string> images)
        {
            var tradesmanImages = new List<TradesManImages>();

            foreach (var image in images)
            {
                tradesmanImages.Add(new TradesManImages
                {
                    ImageUrl = image,
                    TradesmanId = tradesManId,
                });
            }

            _context.TradesManImages.AddRange(tradesmanImages);
            await _context.SaveChangesAsync();
        }
    }
}
