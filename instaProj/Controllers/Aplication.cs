using Microsoft.AspNetCore.Mvc;

namespace instaProj.Controllers
{
    public class Aplication : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
