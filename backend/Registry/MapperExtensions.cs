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
        public static ConversationDTO ToConversationDTO(this Conversation conversation, Guid currentUserId)
        {
            User otherUser;
            if (conversation.User1.Id == currentUserId)
            {
                otherUser = conversation.User2;
            }
            else
            {
                otherUser = conversation.User1;
            }
            var lastMessage = conversation.Messages.MaxBy(m => m.Sent);

            return new ConversationDTO
            {
                Id = conversation.Id,
                With = otherUser.ToConversationUserDTO(),
                LastMessage = lastMessage?.ToMessageDTO(currentUserId),
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

        public static MessageDTO ToMessageDTO(this Message message, Guid currentUserId)
        {
            return new MessageDTO
            {
                Id = message.Id,
                From = message.From.ToConversationUserDTO(),
                Text = message.Text,
                IsMe = message.From.Id == currentUserId,
            };
        }

        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        public static partial FindTradesManDTO ToFindTradesManDTO(this User user);

        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        public static partial SendMessageResponse ToSendMessageResponse(this Message user);
    }
}
