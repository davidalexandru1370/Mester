using Microsoft.EntityFrameworkCore;
using Registry.DTO;
using Registry.DTO.Requests;
using Registry.DTO.Responses;
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
            var r = await _context.Users.Include(x => x.TradesManProfile)
                .ThenInclude(x => x.Specialities)
                .Where(x => x.TradesManProfile != null)
                .FirstAsync(x => x.Id == id);

            if (r is null) return null;

            return new TradesManInfoPageDTO
            {
                Id = r.Id,
                Name = r.Name,
                City = r.TradesManProfile!.City,
                County = r.TradesManProfile!.County,
                Description = r.TradesManProfile!.Description,
                Specialities = r.TradesManProfile.Specialities.Select(x => new SpecialityDTO
                {
                    SpecialityId = x.SpecialityId,
                    TradesManId = x.TradesManId,
                    Price = x.Price,
                    Type = x.Speciality.Type,
                    UnitOfMeasure = x.UnitOfMeasure,
                    ImageUrl = x.Speciality.ImageUrl,
                }).ToList()
            };
        }

        // TODO: return something
        public async Task CreateRequest(User user, User tradesMan, string description)
        {
            var request = new ClientRequest
            {
                From = user,
                To = tradesMan,
                Description = description
            };
            await _context.ClientRequests.AddAsync(request);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ConversationDTO>> GetConversations(User user)
        {
            var r = _context.Conversations
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Include(c => c.Messages)
                .Where(c => c.User1.Id == user.Id || c.User2.Id == user.Id)
                .AsAsyncEnumerable();
            List<ConversationDTO> conversations = [];
            await foreach (var c in r)
            {
                var dto = c.ToConversationDTO(user.Id);
                if (dto.LastMessage is null) continue;
                conversations.Add(dto);
            }
            return conversations;
        }

        public async Task<List<MessageDTO>?> GetMessages(User user, Guid conversationId)
        {
            var conversation = await _context.Conversations
                .Include(c => c.Messages)
                .Include(c => c.User1)
                .Include(c => c.User2)
                // the second if is only used to make sure the user doesn't get messages from another user
                .Where(c => c.Id == conversationId && (c.User1.Id == user.Id || c.User2.Id == user.Id))
                .FirstOrDefaultAsync();
            if (conversation is null) return null;

            return conversation.Messages
                .Select(m => m.ToMessageDTO(user.Id)).ToList();
        }

        public async Task<SendMessageResponse> SendMessage(User user, Guid conversationId, SendMessageRequest sendMessage)
        {
            var conversation = await _context.Conversations.FindAsync(conversationId) ?? throw new ServiceException("Conversation was not found");
            var message = new Message
            {
                From = user,
                Sent = DateTime.Now,
                Text = sendMessage.Text,
                Conversation = conversation,
            };
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
            return message.ToSendMessageResponse();
        }

        public async Task<ConversationDTO> GetOrCreateConversation(User user, Guid withUserId)
        {
            var user1Id = user.Id;
            var user2Id = withUserId;
            if (user1Id > user2Id)
            {
                var tmp = user1Id;
                user1Id = user2Id;
                user2Id = tmp;
            }
            var conversation = await _context.Conversations
                .Include(c => c.User1)
                .Include(c => c.User2)
                .FirstOrDefaultAsync(c => c.User1.Id == user1Id && c.User2.Id == user2Id);
            if (conversation is not null)
            {
                return conversation.ToConversationDTO(user.Id);
            }
            var user1 = await _context.Users.FirstOrDefaultAsync(u => u.Id == user1Id) ?? throw new ServiceException($"User id {user1Id} doesn't exist");
            var user2 = await _context.Users.FirstOrDefaultAsync(u => u.Id == user2Id) ?? throw new ServiceException($"User id {user1Id} doesn't exist");
            var c = new Conversation
            {
                User1 = user1,
                User2 = user2,
            };
            await _context.Conversations.AddAsync(c);
            await _context.SaveChangesAsync();
            return c.ToConversationDTO(user.Id);
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
    }
}
