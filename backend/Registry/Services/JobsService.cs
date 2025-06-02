using Microsoft.EntityFrameworkCore;
using Registry.DTO;
using Registry.DTO.Requests;
using Registry.DTO.Responses;
using Registry.Errors;
using Registry.Errors.Services;
using Registry.Models;
using Registry.Repository;
using Registry.Services.Interfaces;
using Registry.utils;

namespace Registry.Services
{
    public class JobsService : IJobsService
    {
        // A bit lazy to create repo for everything...should we use just the context?
        private readonly TradesManDbContext _context;
        private readonly IImageService _imageService;
        public JobsService(TradesManDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task<List<ConversationDTO>> GetConversations(User user)
        {
            var r = await _context.Conversations
                .Include(c => c.Request)
                .ThenInclude(r => r.InitiatedBy)
                .Include(c => c.Request)
                .ThenInclude(c => c.JobApproved)
                .ThenInclude(c => c.Bills)
                .Include(c => c.TradesMan)
                .Include(c => c.Messages)
                .Where(c => c.Request.InitiatedById == user.Id || c.TradesMan.Id == user.Id)
                .Select(c => c.ToConversationDTO(user.Id))
                .ToListAsync();

            r.Sort((a, b) =>
            {
                var aLastMessageTime = a.ClientRequest.RequestedOn;
                if (a.LastMessage is not null && a.LastMessage.Sent > aLastMessageTime) aLastMessageTime = a.LastMessage.Sent;

                var bLastMessageTime = b.ClientRequest.RequestedOn;
                if (b.LastMessage is not null && b.LastMessage.Sent > aLastMessageTime) bLastMessageTime = b.LastMessage.Sent;

                return -aLastMessageTime.CompareTo(bLastMessageTime);
            });

            return r;
        }

        public async Task<List<MessageOrResponsesOrBillDTO>> GetMessages(User user, Guid conversationId)
        {
            var conversation = await _context.Conversations
                .Include(c => c.Messages)
                .Include(c => c.Request)
                .ThenInclude(c => c.JobApproved)
                .ThenInclude(c => c!.Bills)
                .Include(c => c.Responses)
                .Include(c => c.TradesMan)
                // the second if is only used to make sure the user doesn't get messages from another user
                .Where(c => c.Id == conversationId && (c.Request.InitiatedById == user.Id || c.TradesMan.Id == user.Id))
                .FirstOrDefaultAsync() ?? throw new NotFoundException();
            var r = conversation.Messages
                .Select(m => m.ToMessageAndResponseDTO(user.Id))
                .Union(conversation.Responses.Select(r => r.ToMessageAndResponseDTO()));
            if (conversation.Request.JobApproved is not null)
            {
                r = r.Union(conversation.Request.JobApproved.Bills.Select(b => b.ToMessageAndResponseDTO()));
            }
            return r
                .OrderBy(r => r.Sent)
                .ToList();
        }

        public async Task<SendMessageResponse> SendMessage(User user, Guid conversationId, SendMessageRequest sendMessage)
        {
            var conversation = await _context.Conversations.FirstOrDefaultAsync(c => c.Id == conversationId) ?? throw new ServiceException("Conversation was not found");
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

        public async Task<ConversationDTO> GetOrCreateConversation(User tradesMan, Guid clientJobRequestId)
        {
            var conversation = await _context.Conversations
                .Include(c => c.Messages)
                .Include(c => c.Responses)
                .Include(c => c.Request)
                .ThenInclude(c => c.InitiatedBy)
                .Include(c => c.Request)
                .ThenInclude(c => c.JobApproved)
                .FirstOrDefaultAsync(c => c.Request.Id == clientJobRequestId && c.TradesMan.Id == tradesMan.Id);

            if (conversation is not null)
            {
                return conversation.ToConversationDTO(tradesMan.Id);
            }
            var request = await _context.ClientRequests
                .Include(r => r.InitiatedBy)
                .FirstOrDefaultAsync(r => r.Id == clientJobRequestId) ?? throw new ServiceException("Invalid client job request id");
            var c = new Conversation
            {
                RequestId = request.Id,
                TradesManId = tradesMan.Id,
                TradesMan = tradesMan,
                Request = request,
            };
            var r = await _context.Conversations.AddAsync(c);
            await _context.SaveChangesAsync();
            return r.Entity.ToConversationDTO(tradesMan.Id);
        }

        public async Task<ClientJobRequestDTO> CreateClientRequest(User user, CreateClientJobRequest request)
        {
            var jobRequest = new ClientJobRequest
            {
                Title = request.Title,
                Description = request.Description,
                ShowToEveryone = request.ShowToEveryone,
                StartDate = request.StartDate,
                InitiatedById = user.Id,
                InitiatedBy = user,
                ImagesUrl = []
            };
            var entity = await _context.ClientRequests.AddAsync(jobRequest);
            await _context.SaveChangesAsync();
            return jobRequest.ToClientJobRequestDTO();
        }

        public async Task<ConversationDTO> SendClientRequestToTradesMan(User client, Guid clientRequestId, Guid tradesManId)
        {
            var r = await _context.Conversations.Where(c => c.TradesManId == tradesManId && c.RequestId == clientRequestId).FirstOrDefaultAsync();
            if (r is not null)
            {
                throw new ConflictException("Conversation already exists");
            }
            var request = await _context.ClientRequests
                .Include(r => r.InitiatedBy)
                .Include(r => r.JobApproved)
                .FirstOrDefaultAsync(r => r.Id == clientRequestId) ?? throw new NotFoundException("The client request id was not found");
            var conversation = new Conversation
            {
                RequestId = clientRequestId,
                Request = request,
                TradesManId = tradesManId,

            };
            await _context.Conversations.AddAsync(conversation);
            await _context.SaveChangesAsync();
            conversation = await _context.Conversations.Where(c => c.Id == conversation.Id).Include(c => c.TradesMan).FirstOrDefaultAsync();
            return conversation.ToConversationDTO(client.Id);
        }

        public async Task<ClientJobRequestDTO> UpdateClientRequest(User user, Guid clientRequestId, UpdateClientJobRequest request)
        {
            var r = await _context.ClientRequests.Include(r => r.JobApproved).FirstOrDefaultAsync(r => r.Id == clientRequestId && r.InitiatedById == user.Id) ?? throw new NotFoundException();
            if (request.Title is not null) r.Title = request.Title;
            if (request.Description is not null) r.Description = request.Description;
            if (request.ShowToEveryone is not null) r.ShowToEveryone = request.ShowToEveryone.Value;
            if (request.Open is not null) r.Open = request.Open.Value;
            if (request.IncludeStartDate ?? false) r.StartDate = request.StartDate;
            List<string> urlArray = new List<string>();
            if (request.ImagesUrl is not null)
                foreach (var i in request.ImagesUrl)
                {
                    string url = await _imageService.UploadImage(new Base64Stream(i).AsStream());
                    urlArray.Add(url);
                }
            r.ImagesUrl = urlArray;
            _context.ClientRequests.Update(r);
            await _context.SaveChangesAsync();
            r.InitiatedBy = user;
            return r.ToClientJobRequestDTO();
        }

        public async Task<TradesManJobResponseDTO> AddTradesManJobResponse(User tradesMan, Guid conversationId, CreateTradesManJobResponse request)
        {
            var conversation = await _context.Conversations.Include(c => c.Request).FirstOrDefaultAsync(c => c.Id == conversationId) ?? throw new NotFoundException();
            if (!conversation.Request.Open)
            {
                throw new ServiceException("The request is not open. It can't accept values");
            }
            if (conversation.Request.JobApprovedId.HasValue)
            {
                throw new ServiceException("The request is already approved");
            }
            // TODO: maybe we can add another check that this job is either open or the tradesman has a conversation with the user
            var jobResponse = new TradesManJobResponse
            {
                ConversationId = conversationId,
                Conversation = conversation,
                AproximationEndDate = request.AproximationEndDate,
                WorkmanshipAmount = request.WorkmanShipAmount,
            };
            _context.TradesManJobResponses.Add(jobResponse);
            await _context.SaveChangesAsync();
            return jobResponse.ToTradesManJobResponseDTO();
        }

        public async Task AcceptResponse(User user, Guid responseId)
        {
            var response = await _context.TradesManJobResponses
                .Include(r => r.Conversation)
                .ThenInclude(c => c.Request)
                .FirstOrDefaultAsync(r => r.Id == responseId && r.Conversation.Request.InitiatedById == user.Id) ?? throw new NotFoundException();
            if (response.Conversation.Request.JobApprovedId.HasValue) throw new ConflictException("Request was already accepted");
            var job = new Job
            {
                StartDate = DateTime.Now,
                TradesManJobResponse = response,
                TradesManJobResponseId = responseId,
            };
            await _context.Jobs.AddAsync(job);
            var request = response.Conversation.Request;
            request.JobApproved = job;
            request.JobApprovedId = job.Id;
            _context.ClientRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task EndJob(User tradesMan, Guid jobId, DateTime endDate)
        {
            var r = await _context.Jobs
                .Include(j => j.TradesManJobResponse)
                .ThenInclude(r => r.Conversation)
                .FirstOrDefaultAsync(j => j.Id == jobId && j.TradesManJobResponse.Conversation.TradesManId == tradesMan.Id) ?? throw new NotFoundException();

            r.EndDate = endDate;
            _context.Jobs.Update(r);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ClientJobRequestDTO>> AllClientRequests(User user)
        {
            return await _context.ClientRequests
                .Include(r => r.JobApproved)
                .Include(r => r.InitiatedBy)
                .Where(r => r.InitiatedById == user.Id)
                .Select(r => r.ToClientJobRequestDTO())
                .ToListAsync();
        }

        public async Task<List<ConversationWithLastOfferDTO>> GetConversationsLastOffer(User user, Guid jobRequestDetails)
        {
            //TODO: check that the job requests are to this user
            var conversations = _context.Conversations
                .Include(c => c.TradesMan)
                .Include(c => c.Responses)
                .Where(c => c.RequestId == jobRequestDetails)
                .AsAsyncEnumerable();

            var conversationsDTO = new List<ConversationWithLastOfferDTO>();
            await foreach (var item in conversations)
            {
                var lastResponse = item.Responses.MaxBy(r => r.Sent);
                new ConversationWithLastOfferDTO
                {
                    Id = item.Id,
                    TradesMan = item.TradesMan.ToConversationUserDTO(),
                    Response = lastResponse?.ToTradesManJobResponseDTO()
                };
            }
            return conversationsDTO;
        }

        public async Task<List<ClientJobRequestDTO>> GetGlobalRequests(User tradesMan)
        {
            var exceptListIds = await _context.Conversations.Where(c => c.TradesManId == tradesMan.Id).Select(r => r.RequestId).ToListAsync();
            return await _context.ClientRequests
                .Include(r => r.InitiatedBy)
                .Include(r => r.JobApproved)
                .Where(r => r.JobApprovedId == null && r.ShowToEveryone)
                .OrderBy(r => r.RequestedOn)
                .Where(r => !exceptListIds.Contains(r.Id))
                .Select(v => v.ToClientJobRequestDTO())
                .ToListAsync();
        }

        public async Task<BillDTO> AddBill(User tradesMan, Guid jobId, CreateBillRequest billRequest)
        {
            var job = await _context.Jobs
                .Include(j => j.TradesManJobResponse)
                .ThenInclude(r => r.Conversation)
                .FirstOrDefaultAsync(j => j.Id == jobId && j.TradesManJobResponse.Conversation.TradesManId == tradesMan.Id) ?? throw new NotFoundException();
            string url = await _imageService.UploadImage(new Base64Stream(billRequest.BillImageBase64).AsStream());
            var bill = new Bill
            {
                Amount = billRequest.Amount,
                Description = billRequest.Description,
                JobId = jobId,
                BillImage = url
            };
            await _context.Bills.AddAsync(bill);
            await _context.SaveChangesAsync();
            return bill.ToBillDTO();
        }

        public async Task<BillDTO> PayBill(User user, Guid billId)
        {
            var bill = await _context.Bills
                // the biggest include possible. This shows that something I did wrong with the schema
                .Include(b => b.Job)
                .ThenInclude(j => j.TradesManJobResponse)
                .ThenInclude(r => r.Conversation)
                .ThenInclude(c => c.Request)
                .FirstOrDefaultAsync(b => b.Id == billId && b.Job.TradesManJobResponse.Conversation.Request.InitiatedById == user.Id)
                ?? throw new NotFoundException();
            if (bill.Paid) throw new ConflictException("bill already paied");
            bill.Paid = true;
            _context.Update(bill);
            await _context.SaveChangesAsync();
            return bill.ToBillDTO();
        }
    }
}
