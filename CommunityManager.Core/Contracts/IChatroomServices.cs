using CommunityManager.Core.Models.Chatroom;

namespace CommunityManager.Core.Contracts
{
    /// <summary>
    /// Abstraction of community service methods
    /// </summary>
    public interface IChatroomServices
    {
        /// <summary>
        /// Gets a chatroom with a specific ID
        /// </summary>
        /// <param name="id">ID of the chatroom</param>
        /// <returns>Chatroom view model</returns>
        Task<ChatroomViewModel> GetChatroomByIdAsync(Guid id);

        /// <summary>
        /// Adds a user to a chatroom
        /// </summary>
        /// <param name="id">ID of the chatroom</param>
        /// <param name="userId">ID of the current user</param>
        Task JoinChatroomAsync(Guid id, string userId);

        /// <summary>
        /// Removes a user from a chatroom
        /// </summary>
        /// <param name="id">ID of the chatroom</param>
        /// <param name="userId">ID of the current user</param>
        Task LeaveChatroomAsync(Guid id, string userId);

        /// <summary>
        /// Checks if the current user is a member of a chatroom
        /// </summary>
        /// <param name="chatroomId">ID of the chatroom</param>
        /// <param name="memberId">ID of the current user</param>
        /// <returns>Boolean</returns>
        Task<bool> CheckChatroomMemberId(Guid chatroomId, string memberId);

        /// <summary>
        /// Creates a message
        /// </summary>
        /// <param name="chatroomId">ID of the chatroom</param>
        /// <param name="userId">ID of the current user</param>
        /// <param name="content">Content of the message</param>
        Task CreateMessageAsync(Guid chatroomId, string userId, string content);
    }
}
