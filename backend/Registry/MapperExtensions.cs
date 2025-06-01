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
            var lastResponse = conversation.Responses.MaxBy(m => m.Sent)?.ToMessageAndResponseDTO(currentUser);

            var last = lastMessage;
            if (lastMessage is not null && lastResponse is not null && lastResponse.Sent > lastMessage.Sent)
            {
                last = lastResponse;
            }
            return new ConversationDTO
            {
                Id = conversation.Id,
                ClientRequest = conversation.Request.ToClientJobRequestDTO(),
                TradesMan = conversation.TradesMan.ToConversationUserDTO(),
                LastMessage = last
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

        public static MessageOrResponsesDTO ToMessageAndResponseDTO(this Message message, Guid currentUser)
        {
            return new MessageOrResponsesDTO
            {
                IsMe = message.FromId == currentUser,
                Message = message.ToMessageDTO(),
                Seen = message.Seen,
                Sent = message.Sent,
            };
        }

        public static MessageOrResponsesDTO ToMessageAndResponseDTO(this TradesManJobResponse response, Guid currentUser)
        {
            return new MessageOrResponsesDTO
            {
                IsMe = response.TradesManId == currentUser,
                ClientRequestResponse = response.ToTradesManJobResponseDTO(),
                Seen = response.Seen,
                Sent = response.Sent,
            };
        }

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
                StartDate = request.StartDate
            };
        }

        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        public static partial MessageResponse ToMessageDTO(this Message message);

        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        public static partial TradesManJobResponseDTO ToTradesManJobResponseDTO(this TradesManJobResponse response);

        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        public static partial FindTradesManDTO ToFindTradesManDTO(this User user);

        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        public static partial SendMessageResponse ToSendMessageResponse(this Message user);
    }
}
