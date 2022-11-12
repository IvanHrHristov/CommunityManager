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
                TempData[ErrorMessage] = "Incorrect product ID";

                return RedirectToAction("Open", "Community", new { id = communityId });
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
                TempData[ErrorMessage] = "Incorrect product ID";

                return RedirectToAction("Open", "Community", new { id = communityId });
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
                TempData[ErrorMessage] = "Incorrect product ID";

                return RedirectToAction("Open", "Community", new { id = communityId });
            }
        }
    }
}
