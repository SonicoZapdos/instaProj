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
        public IActionResult Login()
        {
            return View();
        }
        /* if (arqRecebido == null && imagem.imagemLink != "")
            {
                WebClient cli = new WebClient();

                var idVideo = imagem.imagemLink.Substring(32);

                var imgBytes = cli.DownloadData($"http://img.youtube.com/vi/{idVideo}/0.jpg");

                string fileName = geraNomeRandomizado(25);

                System.IO.File.WriteAllBytes(@"wwwroot/imgMINI/" + fileName + ".jpg", imgBytes);

                imagem.imagemNomeArmazenamento = fileName + ".jpg";
                imagem.imagemNomeArquivo = fileName + ".jpg";

                _context.Add(imagem);
                _context.SaveChanges();

                return RedirectToAction("Index");
            } */
    }
}
