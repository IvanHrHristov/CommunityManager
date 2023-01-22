using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.Chatroom;
using CommunityManager.Core.Models.User;
using CommunityManager.Infrastructure.Data;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace CommunityManager.Core.Services
{
    /// <summary>
    /// Implementation of community service methods
    /// </summary>
    public class ChatroomServices : IChatroomServices
    {
        /// <summary>
        /// Repository providing access to the database 
        /// </summary>
        private readonly IRepository repository;

        public ChatroomServices(IRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Checks if the current user is a member of a chatroom
        /// </summary>
        /// <param name="chatroomId">ID of the chatroom</param>
        /// <param name="memberId">ID of the current user</param>
        /// <returns>Boolean</returns>
        public async Task<bool> CheckChatroomMemberId(Guid chatroomId, string memberId)
        {
            var chatroomMembers = repository.AllReadonly<ChatroomMember>()
                .Where(cm => cm.ChatroomId == chatroomId);

            return await chatroomMembers.AnyAsync(cm => cm.ApplicationUserId == memberId);
        }

        /// <summary>
        /// Creates a message
        /// </summary>
        /// <param name="chatroomId">ID of the chatroom</param>
        /// <param name="userId">ID of the current user</param>
        /// <param name="content">Content of the message</param>
        public async Task CreateMessageAsync(Guid chatroomId, string userId, string content)
        {
            var entity = new Message()
            {
                ChatroomId = chatroomId,
                SenderId = userId,
                Content = content,
                CreatedOn = DateTime.Now
            };

            await repository.AddAsync(entity);
            await repository.SaveChangesAsync();
        }

        /// <summary>
        /// Gets a chatroom with a specific ID
        /// </summary>
        /// <param name="id">ID of the chatroom</param>
        /// <returns>Chatroom view model</returns>
        public async Task<ChatroomViewModel> GetChatroomByIdAsync(Guid id)
        {
            var entity = await repository.All<Chatroom>()
                .Include(c => c.Messages)
                .ThenInclude(m => m.Sender)
                .Include(c => c.ChatroomsMembers)
                .ThenInclude(chm => chm.ApplicationUser)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null)
            {
                return new ChatroomViewModel();
            }

            return new ChatroomViewModel()
            {
                Id = id,
                Name = entity.Name,
                Members = entity.ChatroomsMembers.Select(chm => new UserViewModel()
                {
                    Id = chm.ApplicationUserId,
                    Name = chm.ApplicationUser.UserName
                }).ToList(),
                Messages = entity?.Messages?.Select(m => new MessageViewModel()
                {
                    Id = m.Id,
                    Content = m.Content,
                    SenderId = m.SenderId,
                    SenderUserName = m.Sender.UserName,
                    ChatroomId = m.ChatroomId,
                    CreatedOn = m.CreatedOn
                })
                .OrderBy(m => m.CreatedOn)
                .ToList()
            };
        }

        /// <summary>
        /// Adds a user to a chatroom
        /// </summary>
        /// <param name="id">ID of the chatroom</param>
        /// <param name="userId">ID of the current user</param>
        public async Task JoinChatroomAsync(Guid id, string userId)
        {
            var chatroomMember = new ChatroomMember()
            {
                ApplicationUserId = userId,
                ChatroomId = id
            };

            await repository.AddAsync(chatroomMember);
            await repository.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a user from a chatroom
        /// </summary>
        /// <param name="id">ID of the chatroom</param>
        /// <param name="userId">ID of the current user</param>
        public async Task LeaveChatroomAsync(Guid id, string userId)
        {
            var chatroomMember = await repository.All<ChatroomMember>()
                .FirstAsync(chm => chm.ChatroomId == id && chm.ApplicationUserId == userId);

            repository.Delete(chatroomMember);
            await repository.SaveChangesAsync();
        }
    }
}
