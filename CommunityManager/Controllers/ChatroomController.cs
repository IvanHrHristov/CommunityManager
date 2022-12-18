using CommunityManager.Core.Contracts;
using CommunityManager.Extensions;
using CommunityManager.Hubs;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CommunityManager.Controllers
{
    /// <summary>
    /// Controller to manage communities
    /// </summary>
    [Authorize]
    public class ChatroomController : Controller
    {
        /// <summary>
        /// Repository providing access to the database 
        /// </summary>
        private readonly IRepository repository;
        /// <summary>
        /// Service providing methods to manage chatrooms
        /// </summary>
        private readonly IChatroomServices chatroomService;
        /// <summary>
        /// Service providing methods to manage communities
        /// </summary>
        private readonly ICommunityServices communityService;
        /// <summary>
        /// Providing access to the UserManager 
        /// </summary>
        private readonly UserManager<ApplicationUser> userManager;

        public ChatroomController(
            IRepository repository,
            IChatroomServices chatroomService,
            ICommunityServices communityService,
            UserManager<ApplicationUser> userManager)
        {
            this.repository = repository;
            this.chatroomService = chatroomService;
            this.communityService = communityService;
            this.userManager = userManager;
        }

        /// <summary>
        /// Opens a chatroom
        /// </summary>
        /// <param name="id">ID of the chatroom</param>
        /// <param name="communityId">ID of the community</param>
        /// <returns>Chatroom view model</returns>
        public async Task<IActionResult> Open(Guid id, Guid communityId)
        {
            var user = await userManager.GetUserAsync(User);

            ViewBag.currentUserId = user.Id;

            if (!await communityService.CheckCommunityMemberId(communityId, user.Id))
            {
                return RedirectToAction("Error404", "Home");
            }

            if (!await chatroomService.CheckChatroomMemberId(id, user.Id))
            {
                return RedirectToAction("Error404", "Home");
            }

            var model = await chatroomService.GetChatroomByIdAsync(id);

            if (model.Name == null)
            {
                return RedirectToAction("Error404", "Home");
            }

            ViewBag.UserName = user.UserName;
            ViewBag.ChatroomId = id;
            ViewBag.CommunityId = communityId;

            return View(model);
        }

        /// <summary>
        /// Adds a user to a chatroom
        /// </summary>
        /// <param name="id">ID of the chatroom</param>
        /// <param name="communityId">ID of the community</param>
        /// <returns>Redirects the user to a view showing the community</returns>
        public async Task<IActionResult> Join(Guid id, Guid communityId)
        {
            var userId = User.Id();

            if (!(await communityService.CheckCommunityMemberId(communityId, userId)))
            {
                return RedirectToAction("Error404", "Home");
            }

            if (await chatroomService.CheckChatroomMemberId(id, userId))
            {
                return RedirectToAction("Error404", "Home");
            }

            await chatroomService.JoinChatroomAsync(id, userId);

            return RedirectToAction("AllChatrooms", "Community", new { id = communityId });
        }

        /// <summary>
        /// Removes a user from a chatroom
        /// </summary>
        /// <param name="id">ID of the chatroom</param>
        /// <param name="communityId">ID of the community</param>
        /// <returns>Redirects the user to a view showing the community</returns>
        public async Task<IActionResult> Leave(Guid id, Guid communityId)
        {
            var userId = User.Id();

            if (!(await communityService.CheckCommunityMemberId(communityId, userId)))
            {
                return RedirectToAction("Error404", "Home");
            }

            if (!(await chatroomService.CheckChatroomMemberId(id, userId)))
            {
                return RedirectToAction("Error404", "Home");
            }

            await chatroomService.LeaveChatroomAsync(id, userId);

            return RedirectToAction("AllChatrooms", "Community", new { id = communityId });
        }
    }
}
