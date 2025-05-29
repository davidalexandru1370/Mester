using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Registry.DTO;
using Registry.Errors.Services;
using Registry.Models;
using Registry.Repository;
using System.Runtime.CompilerServices;

namespace Registry.Services
{
    // TODO: this should be in DTO or contracts?
    public record FilterListTradesMen(List<string>? Specialties);

    [EnableCors("allPolicy")]
    public class TradesManService
    {
        // A bit lazy to create repo for everything...should we use just the context?
        private readonly TradesManDbContext _context;

        public TradesManService(TradesManDbContext context)
        {
            _context = context;
        }


        public async Task<List<string>> GetSpecialities(CancellationToken token = default)
        {
            return await _context.Specialties.Select(s => s.Type).ToListAsync(token);
        }

        public async Task<Specialty?> FindSpeciality(string Type, CancellationToken token = default)
        {
            return await _context.Specialties.FirstOrDefaultAsync(x => x.Type == Type, token);
        }

        public async Task<List<Specialty>> GetSpecialitiesByName(IList<string> specialitiesTypeNames, CancellationToken token = default)
        {
            var specialities = new List<Specialty>();
            var invalidSpecialities = new List<string>();
            foreach (var specialityName in specialitiesTypeNames)
            {
                var speciality = await FindSpeciality(specialityName, token);
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

        public async Task UpdateTradesManProfile(User user, TradesManDTO tradesManDTO, CancellationToken token = default)
        {
            var specialities = await GetSpecialitiesByName(tradesManDTO.Specialties, token);

            user.TradesManProfile = new TradesMan { Specialties = specialities, Description = tradesManDTO.Description };
            _context.Update(user);
            await _context.SaveChangesAsync(token);
        }

        public async Task<List<TradesManListDTO>> GetTradesManList(FilterListTradesMen filter, CancellationToken token = default)
        {
            //TODO: add sorting based on rating
            var query = _context.Users.Include(x => x.TradesManProfile)
                .ThenInclude(x => x.Specialties)
                .Where(x => x.TradesManProfile != null);
            if (filter.Specialties is not null)
            {
                query = query.Where(x => x.TradesManProfile!.Specialties.Select(x => x.Type)
                    .Any(x => filter.Specialties.Any(y => x == y)));
            }
            return await query
                .Select(x => new TradesManListDTO
                {
                    Id = x.Id,
                    Description = x.TradesManProfile!.Description,
                    Name = x.Name,
                    Specialities = x.TradesManProfile!.Specialties.Select(x => x.Type).ToList()
                })
                .ToListAsync(token);
        }

        public async Task<TradesManInfoPageDTO?> GetTradesManInfo(Guid id)
        {
            var r = await _context.Users.Include(x => x.TradesManProfile)
                .ThenInclude(x => x.Specialties)
                .Where(x => x.TradesManProfile != null)
                .FirstAsync(x => x.Id == id);

            if (r is null) return null;

            return new TradesManInfoPageDTO
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.TradesManProfile!.Description,
                Specialities = r.TradesManProfile.Specialties.Select(x => x.Type).ToList()
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

        public async IAsyncEnumerable<ConversationDTO> GetConversations(User user, [EnumeratorCancellation] CancellationToken token = default)
        {
            var r = _context.Conversations
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Include(c => c.Messages)
                .Where(c => c.User1.Id == user.Id || c.User2.Id == user.Id)
                .AsAsyncEnumerable()
                .WithCancellation(token);

            await foreach (var c in r)
            {
                var dto = c.ToConversationDTO(user.Id);
                if (dto is null) continue;
                yield return dto;
            }
        }

        public async Task<List<MessageDTO>?> GetMessages(User user, Guid conversationId, CancellationToken token = default)
        {
            var conversation = await _context.Conversations
                .Include(c => c.Messages)
                // the second if is only used to make sure the user doesn't get messages from another user
                .Where(c => c.Id == conversationId && (c.User1.Id == user.Id || c.User2.Id == user.Id))
                .FirstOrDefaultAsync(token);
            if (conversation is null) return null;

            return conversation.Messages.Select(m => m.ToMessageDTO(user.Id)).ToList();
        }

        public async Task SendMessage(User user, SendMessageDTO sendMessage, CancellationToken token = default)
        {
            var conversation = await _context.Conversations.FindAsync(sendMessage.ConversationId, token) ?? throw new ServiceException("Conversation was not found");
            var message = new Message
            {
                From = user,
                Sent = DateTime.Now,
                Text = sendMessage.Text,
                Conversation = conversation,
            };
            await _context.Messages.AddAsync(message, token);
            await _context.SaveChangesAsync(token);
        }
    }
}
