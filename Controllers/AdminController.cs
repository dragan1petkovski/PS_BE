using Microsoft.AspNetCore.Mvc;

namespace be.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
