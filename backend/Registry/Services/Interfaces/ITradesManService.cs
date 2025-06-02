using Registry.DTO;
using Registry.Models;

namespace Registry.Services.Interfaces;

public interface ITradesManService
{
    Task<List<string>> GetSpecialities();
    Task<Speciality?> FindSpeciality(string Type);
    Task<List<Speciality>> GetSpecialitiesByName(IList<string> specialitiesTypeNames);
    Task UpdateTradesManProfile(User user, TradesManDTO tradesManDTO);
    Task<List<TradesManListDTO>> GetTradesManList(FilterListTradesMen filter);
    Task<TradesManInfoPageDTO?> GetTradesManInfo(Guid id);
    Task<Speciality> AddSpecialty(Speciality speciality);
    Task<List<Speciality>> AddSpecialitiesBulk(List<Speciality> specialities);
    Task<List<FindTradesManDTO>> FindTradesMen(string pattern, int limit);
    Task AddWorkorderImages(Guid tradesmanId, List<IFormFile> images);
    Task AddWorkorderImages(Guid tradesmanId, List<string> images);

}