using Microsoft.EntityFrameworkCore;
using Registry.DTO;
using Registry.DTO.Requests;
using Registry.DTO.Responses;
using Registry.Errors;
using Registry.Errors.Services;
using Registry.Models;
using Registry.Repository;
using Registry.Services.Interfaces;

namespace Registry.Services
{
    public class JobsService : IJobsService
    {
        // A bit lazy to create repo for everything...should we use just the context?
        private readonly TradesManDbContext _context;

        public JobsService(TradesManDbContext context)
        {
            _context = context;
        }

        public async Task<List<ConversationDTO>> GetConversations(User user)
        {
            var r = _context.Conversations
                .Include(c => c.Request)
                .Include(c => c.TradesMan)
                .Include(c => c.Messages)
                .Where(c => c.Request.InitiatedById == user.Id || c.TradesMan.Id == user.Id)
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

        public async Task<List<MessageAndResponsesDTO>?> GetMessages(User user, Guid conversationId)
        {
            var conversation = await _context.Conversations
                .Include(c => c.Messages)
                .Include(c => c.Request)
                .Include(c => c.TradesMan)
                // the second if is only used to make sure the user doesn't get messages from another user
                .Where(c => c.Id == conversationId && (c.Request.InitiatedById == user.Id || c.TradesMan.Id == user.Id))
                .FirstOrDefaultAsync();
            if (conversation is null) return null;

            return conversation.Messages
                .Select(m => m.ToMessageAndResponseDTO(user.Id))
                .Union(conversation.Responses.Select(r => r.ToMessageAndResponseDTO(user.Id)))
                .ToList();
        }

        public async Task<SendMessageResponse> SendMessage(User user, Guid conversationId, SendMessageRequest sendMessage)
        {
            var conversation = await _context.Conversations.FindAsync(conversationId) ?? throw new ServiceException("Conversation was not found");
            var message = new Message
            {
                FromId = user.Id,
                From = user,
                Sent = DateTime.Now,
                Text = sendMessage.Text,
                Conversation = conversation,
            };
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
            return message.ToSendMessageResponse();
        }

        public async Task<ConversationDTO> GetOrCreateConversation(User user, Guid clientJobRequestId, Guid tradesManId)
        {
            var conversation = await _context.Conversations
                .Include(c => c.Messages)
                .Include(c => c.Responses)
                .FirstOrDefaultAsync(c => c.Request.Id == clientJobRequestId && c.TradesMan.Id == tradesManId);

            if (conversation is not null)
            {
                return conversation.ToConversationDTO(user.Id);
            }
            var request = await _context.ClientRequests.FirstOrDefaultAsync(r => r.Id == clientJobRequestId) ?? throw new ServiceException("Invalid client job request id");
            var tradesMan = await _context.Users.FirstOrDefaultAsync(u => u.Id == tradesManId && u.TradesManProfile != null) ?? throw new ServiceException($"Tradesman with id {tradesManId} doesn't exist");
            var c = new Conversation
            {
                RequestId = request.Id,
                TradesManId = tradesManId,
            };
            await _context.Conversations.AddAsync(c);
            await _context.SaveChangesAsync();
            return c.ToConversationDTO(user.Id);
        }

        public async Task<Guid> CreateClientRequest(User user, CreateClientJobRequest request)
        {
            var jobRequest = new ClientJobRequest
            {
                Title = request.Title,
                Description = request.Description,
                ShowToEveryone = request.ShowToEveryone,
                StartDate = request.StartDate,
                InitiatedById = user.Id
            };
            await _context.ClientRequests.AddAsync(jobRequest);
            await _context.SaveChangesAsync();
            return jobRequest.Id;
        }

        public async Task<ConversationDTO> SendClientRequestToConversation(User client, Guid clientRequestId, Guid tradesManId)
        {
            var r = await _context.Conversations.Where(c => c.TradesManId == tradesManId && c.RequestId == clientRequestId).FirstOrDefaultAsync();
            if (r is not null)
            {
                throw new ConflictException("Conversation already exists");
            }
            var conversation = new Conversation
            {
                RequestId = clientRequestId,
                TradesManId = tradesManId,
            };
            await _context.Conversations.AddAsync(conversation);
            await _context.SaveChangesAsync();
            return conversation.ToConversationDTO(client.Id);
        }

        public async Task UpdateClientRequest(User user, Guid clientRequestId, UpdateClientJobRequest request)
        {
            var r = await _context.ClientRequests.FirstOrDefaultAsync(r => r.Id == clientRequestId && r.InitiatedById == user.Id) ?? throw new NotFoundException();
            if (request.Title is not null) r.Title = request.Title;
            if (request.Description is not null) r.Description = request.Description;
            if (request.ShowToEveryone is not null) r.ShowToEveryone = request.ShowToEveryone.Value;
            if (request.Open is not null) r.Open = request.Open.Value;
            if (request.IncludeStartDate ?? false) r.StartDate = request.StartDate;
            _context.ClientRequests.Update(r);
            await _context.SaveChangesAsync();
        }

        public async Task<TradesManJobResponseDTO> AddTradesManJobResponse(User tradesMan, CreateTradesManJobResponse request)
        {
            var clientJobRequest = await _context.ClientRequests.FirstOrDefaultAsync(r => r.Id == request.ClientJobRequest) ?? throw new NotFoundException();
            if (!clientJobRequest.Open)
            {
                throw new ServiceException("The request is not open. It can't accept values");
            }
            if (clientJobRequest.JobApprovedId.HasValue)
            {
                throw new ServiceException("The request is already approved");
            }
            // TODO: maybe we can add another check that this job is either open or the tradesman has a conversation with the user
            var jobResponse = new TradesManJobResponse
            {
                ClientJobRequestId = clientJobRequest.Id,
                ClientJobRequest = clientJobRequest,
                AproximationEndDate = request.AproximationEndDate,
                WorkmanshipAmount = request.WorkmanShipAmount,
                TradesManId = tradesMan.Id,
            };
            _context.TradesManJobResponses.Add(jobResponse);
            await _context.SaveChangesAsync();
            return jobResponse.ToTradesManJobResponseDTO();
        }

        public async Task AcceptResponse(User user, Guid requestId, Guid responseId)
        {
            var request = await _context.ClientRequests.FirstOrDefaultAsync(r => r.Id == requestId && r.InitiatedById == user.Id) ?? throw new NotFoundException();
            var response = await _context.TradesManJobResponses.FirstOrDefaultAsync(r => r.Id == responseId) ?? throw new NotFoundException();

            var job = new Job
            {
                JobRequest = request,
                StartDate = DateTime.Now,
                TradesManJobResponse = response
            };
            await _context.Jobs.AddAsync(job);
            request.JobApproved = job;
            request.JobApprovedId = job.Id;
            _context.ClientRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task EndJob(User tradesMan, Guid jobId, DateTime endDate)
        {
            var r = await _context.Jobs
                .Include(j => j.TradesManJobResponse)
                .FirstOrDefaultAsync(j => j.Id == jobId && j.TradesManJobResponse.TradesManId == tradesMan.Id) ?? throw new NotFoundException();

            r.EndDate = endDate;
            _context.Jobs.Update(r);
            await _context.SaveChangesAsync();
        }
    }
}
