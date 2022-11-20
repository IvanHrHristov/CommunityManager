using CommunityManager.Core.Contracts;
using CommunityManager.Extensions;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

            //var currentUser = await userManager.GetUserAsync(User);
            //var messages = await repository.All<Message>().ToListAsync();

            //ViewBag.CurrentUserName = currentUser.UserName;

            return View(model);
        }

        //public async Task<IActionResult> Create(Message message)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        message.Sender.UserName = User?.Identity?.Name;
        //        var sender = await userManager.GetUserAsync(User);
        //        message.SenderId = sender.Id;
        //        await repository.AddAsync(message);
        //        await repository.SaveChangesAsync();
        //        return Ok();
        //    }
        //    return BadRequest();
        //}

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
