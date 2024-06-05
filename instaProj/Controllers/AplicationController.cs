using instaProj.Models;
using Microsoft.AspNetCore.Mvc;

namespace instaProj.Controllers
{
    public class AplicationController : Controller
    {
        private readonly Contexto _context;

        public AplicationController(Contexto context)
        {
            _context = context;
        }
        public IActionResult Main()
        {
            return View();
        }
        public IActionResult ForYou()
        {
            return View();
        }
        public IActionResult Menu()
        {
            return View();
        }
        public IActionResult Follows()
        {
            return View();
        }
    }
}
