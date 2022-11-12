using CommunityManager.Core.Models.Chatroom;

namespace CommunityManager.Core.Contracts
{
    public interface IChatroomServices
    {
        Task<ChatroomViewModel> GetChatroomByIdAsync(Guid id);
    }
}
