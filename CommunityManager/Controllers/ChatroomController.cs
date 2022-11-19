using CommunityManager.Core.Contracts;
using CommunityManager.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityManager.Controllers
{
    [Authorize]
    public class ChatroomController : Controller
    {
        private readonly IChatroomServices chatroomService;
        private readonly ICommunityServices communityService;

        public ChatroomController(
            IChatroomServices chatroomService,
            ICommunityServices communityService)
        {
            this.chatroomService = chatroomService;
            this.communityService = communityService;
        }

        public async Task<IActionResult> Open(Guid id, Guid communityId)
        {
            var currentUserId = User.Id();

            ViewBag.currentUserId = currentUserId;

            if (!(await communityService.CheckCommunityMemberId(communityId, currentUserId)))
            {
                var errorMessage = "An error occured while trying to open that chatroom";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }

            if (!(await chatroomService.CheckChatroomMemberId(id, currentUserId)))
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
