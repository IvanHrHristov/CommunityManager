using CommunityManager.Core.Contracts;
using CommunityManager.Extensions;
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


    }
}
