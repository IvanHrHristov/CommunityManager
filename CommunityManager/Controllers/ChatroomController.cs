using Microsoft.AspNetCore.Mvc;

namespace CommunityManager.Controllers
{
    public class ChatroomController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
