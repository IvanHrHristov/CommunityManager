using Microsoft.AspNetCore.Mvc;

namespace CommunityManager.Controllers
{
    public class MarketplaceController : Controller
    {
        public IActionResult All()
        {
            return View();
        }
    }
}
