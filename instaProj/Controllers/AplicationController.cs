using instaProj.Models;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp.Formats.Jpeg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Net;
using System.ComponentModel.DataAnnotations;

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
            if (HttpContext.Session.GetString("USERLOGADO") != null)
            {
                string? userId = HttpContext.Session.GetString("USERLOGADO");

                if (userId != "" || userId == null)
                {
                    User? pessoaLogada = _context.Users.FirstOrDefault(m => m.Id == int.Parse(userId ?? ""));

                    if (pessoaLogada != null)
                    {
                        ViewBag.User = pessoaLogada;
                        ViewBag.Following = null;
                        ViewBag.Followed = null;
                    }
                    return View();
                }
            }
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
