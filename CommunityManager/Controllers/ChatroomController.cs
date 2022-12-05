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
    [Authorize]
    public class ChatroomController : Controller
    {
        private readonly IRepository repository;
        private readonly IChatroomServices chatroomService;
        private readonly ICommunityServices communityService;
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

        public async Task<IActionResult> Open(Guid id, Guid communityId)
        {
            var user = await userManager.GetUserAsync(User);

            ViewBag.currentUserId = user.Id;

            if (!await communityService.CheckCommunityMemberId(communityId, user.Id))
            {
                var errorMessage = "An error occured while trying to open that chatroom";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }

            if (!await chatroomService.CheckChatroomMemberId(id, user.Id))
            {
                var errorMessage = "An error occured while trying to open that chatroom";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }

            var model = await chatroomService.GetChatroomByIdAsync(id);

            if (model.Name == null)
            {
                var errorMessage = "The chatroom you are trying to open does not exist";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }

            ViewBag.UserName = user.UserName;
            ViewBag.ChatroomId = id;
            ViewBag.CommunityId = communityId;

            return View(model);
        }

        public async Task<IActionResult> Join(Guid id, Guid communityId)
        {
            var userId = User.Id();

            if (!(await communityService.CheckCommunityMemberId(communityId, userId)))
            {
                var errorMessage = "An error occured while trying to join that chatroom";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }

            if (await chatroomService.CheckChatroomMemberId(id, userId))
            {
                var errorMessage = "An error occured while trying to join that chatroom";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }

            try
            {
                await chatroomService.JoinChatroomAsync(id, userId);

                return RedirectToAction("Open", "Community", new { id = communityId });
            }
            catch (Exception)
            {
                var errorMessage = "The chatroom you are trying to join does not exist";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }
        }

        public async Task<IActionResult> Leave(Guid id, Guid communityId)
        {
            var userId = User.Id();

            if (!(await communityService.CheckCommunityMemberId(communityId, userId)))
            {
                var errorMessage = "An error occured while trying to leave that chatroom";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }

            if (!(await chatroomService.CheckChatroomMemberId(id, userId)))
            {
                var errorMessage = "An error occured while trying to leave that chatroom";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }

            try
            {
                await chatroomService.LeaveChatroomAsync(id, userId);

                return RedirectToAction("Open", "Community", new { id = communityId });
            }
            catch (Exception)
            {
                var errorMessage = "The chatroom you are trying to leave does not exist";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }
        }
    }
}
