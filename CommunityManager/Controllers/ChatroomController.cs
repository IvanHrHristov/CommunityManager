using CommunityManager.Core.Contracts;
using CommunityManager.Extensions;
using CommunityManager.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Mvc;
using static CommunityManager.Infrastructure.Data.Constants.MessageConstants;

namespace CommunityManager.Controllers
{
    public class ChatroomController : Controller
    {
        private readonly IChatroomServices chatroomService;

        public ChatroomController(IChatroomServices chatroomService)
        {
            this.chatroomService = chatroomService;
        }

        public async Task<IActionResult> Open(Guid id, Guid communityId)
        {
            var currentUserId = User.Id();

            ViewBag.currentUserId = currentUserId;

            var model = await chatroomService.GetChatroomByIdAsync(id);

            if (model.Name == null)
            {
                var errorMessage = "The chatroom you are trying to open does not exist";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }

            return View(model);
        }

        public async Task<IActionResult> Join(Guid id, Guid communityId)
        {
            var userId = User.Id();

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
