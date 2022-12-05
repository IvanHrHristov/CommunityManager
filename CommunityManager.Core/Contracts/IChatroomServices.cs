using CommunityManager.Core.Models.Chatroom;

namespace CommunityManager.Core.Contracts
{
    public interface IChatroomServices
    {
        Task<ChatroomViewModel> GetChatroomByIdAsync(Guid id);

        Task JoinChatroomAsync(Guid id, string userId);

        Task LeaveChatroomAsync(Guid id, string userId);

        Task<bool> CheckChatroomMemberId(Guid chatroomId, string memberId);

        Task CreateMessageAsync(Guid chatroomId, string userId, string content);
    }
}
