using Registry.DTO;
using Registry.DTO.Responses;
using Registry.Models;
using Riok.Mapperly.Abstractions;

namespace Registry
{
    // Setup the Mapperly maper in case somebody wants to use it, but I prefer the manual mapping
    [Mapper]
    public static partial class MapperExtensions
    {
        public static ConversationDTO ToConversationDTO(this Conversation conversation, Guid currentUser)
        {
            var lastMessage = conversation.Messages.MaxBy(m => m.Sent)?.ToMessageAndResponseDTO(currentUser);
            var lastResponse = conversation.Responses.MaxBy(m => m.Sent)?.ToMessageAndResponseDTO();
            var lastBill = conversation.Request.JobApproved?.Bills.MaxBy(b => b.Sent)?.ToMessageAndResponseDTO();

            var last = lastMessage;
            if (last is null) last = lastResponse;
            else if (lastResponse is not null && lastResponse.Sent > last.Sent)
            {
                last = lastResponse;
            }
            if (last is null) last = lastBill;
            else if (lastBill is not null && lastBill.Sent > last.Sent)
            {
                last = lastBill;
            }
            return new ConversationDTO
            {
                Id = conversation.Id,
                ClientRequest = conversation.Request.ToClientJobRequestDTO(),
                TradesMan = conversation.TradesMan.ToConversationUserDTO(),
                LastMessage = last,
            };
        }

        public static ConversationUserDTO ToConversationUserDTO(this User user)
        {
            return new ConversationUserDTO
            {
                Id = user.Id,
                Name = user.Name,
                ImageUrl = user.ImageUrl
            };
        }

        public static MessageOrResponsesOrBillDTO ToMessageAndResponseDTO(this Message message, Guid currentUser)
        {
            return new MessageOrResponsesOrBillDTO
            {
                Message = message.ToMessageDTO(currentUser),
                Seen = message.Seen,
                Sent = message.Sent,
            };
        }

        public static MessageOrResponsesOrBillDTO ToMessageAndResponseDTO(this TradesManJobResponse response)
        {
            return new MessageOrResponsesOrBillDTO
            {
                Response = response.ToTradesManJobResponseDTO(),
                Seen = response.Seen,
                Sent = response.Sent,
            };
        }

        public static MessageOrResponsesOrBillDTO ToMessageAndResponseDTO(this Bill response)
        {
            return new MessageOrResponsesOrBillDTO
            {
                Bill = response.ToBillDTO(),
                Seen = response.Seen,
                Sent = response.Sent,
            };
        }

        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        public static partial BillDTO ToBillDTO(this Bill bill);

        public static ClientJobRequestDTO ToClientJobRequestDTO(this ClientJobRequest request)
        {
            return new ClientJobRequestDTO
            {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                JobApprovedId = request.JobApprovedId,
                Open = request.Open,
                RequestedOn = request.RequestedOn,
                ShowToEveryone = request.ShowToEveryone,
                // TODO: add support for images
                ImagesUrl = new(),
                StartDate = request.StartDate,
                Client = request.InitiatedBy.ToConversationUserDTO(),
                TradesManResponseApproveId = request.JobApproved?.TradesManJobResponseId
            };
        }

        public static MessageResponse ToMessageDTO(this Message message, Guid currentUserId)
        {
            return new MessageResponse
            {
                Id = message.Id,
                Text = message.Text,
                IsMe = message.FromId == currentUserId
            };
        }

        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        public static partial TradesManJobResponseDTO ToTradesManJobResponseDTO(this TradesManJobResponse response);

        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        public static partial FindTradesManDTO ToFindTradesManDTO(this User user);

        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        public static partial SendMessageResponse ToSendMessageResponse(this Message user);
    }
}
