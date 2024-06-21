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
        public IActionResult Main(string page)
        {
            if (HttpContext.Session.GetString("USERLOGADO") != null && int.TryParse(HttpContext.Session.GetString("USERLOGADO"), out int id))
            {
                User? pessoaLogada = _context.Users.FirstOrDefault(m => m.Id == id);

                if (pessoaLogada != null)
                {
                    ViewBag.User = pessoaLogada;
                    ViewBag.Following = _context.Follows.Where(m => m.User_Id_Followed == id);
                    ViewBag.Followed = _context.Follows.Where(m => m.User_Id_Following == id);
                    ViewBag.MainPage = page ?? "ForYou";
                    List<Post> post = _context.Posts.Where(m => m.User_Id == id).ToList() ?? new List<Post>();
                    foreach (Post p in post) 
                    {
                        p.Archives = _context.Archives.Where(m => m.Post_Id == p.Id).ToList();
                        if (_context.Ratings.FirstOrDefault(m => m.Post_Id == p.Id && m.User_Id == id) != null)
                        {
                            p.Rating = true;
                        }
                        else
                        {
                            p.Rating = false;
                        }
                        p.Comment = _context.Comments.Where(m => m.Post_Id == p.Id).ToList() ?? new List<Comment>();
                    }
                    ViewBag.MyPosts = post;
                    if (page == "UpdateUser")
                    {
                        ViewBag.Model = ViewBag.User;
                    }
                    else
                    {
                        ViewBag.Model = null;
                    }
                    post = _context.Posts.Where(m => m.User_Id != id).Include(m => m.User).ToList() ?? new List<Post>();
                    foreach (Post p in post)
                    {
                        p.Archives = _context.Archives.Where(m => m.Post_Id == p.Id).ToList();
                        if (_context.Ratings.FirstOrDefault(m => m.Post_Id == p.Id && m.User_Id == id) != null)
                        {
                            p.Rating = true;
                        }
                        else
                        {
                            p.Rating = false;
                        }
                        p.Comment = _context.Comments.Where(m => m.Post_Id == p.Id).ToList() ?? new List<Comment>();
                    }
                    ViewBag.OtherPosts = post;
                }
                return View();
            }
            return View();
        }

        public IActionResult MyPage()
        {
            if (HttpContext.Session.GetString("USERLOGADO") != null && int.TryParse(HttpContext.Session.GetString("USERLOGADO"), out int id))
            {
                User? pessoaLogada = _context.Users.FirstOrDefault(m => m.Id == id);

                if (pessoaLogada != null)
                {
                    ViewBag.User = pessoaLogada;
                }
                return View();
            }
            return View();
        }
        public IActionResult ForYou()
        {
            return View();
        }

        public IActionResult UpdateUser()
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Favorite(int post)
        {
            if (post != 0 && HttpContext.Session.GetString("USERLOGADO") != null && int.TryParse(HttpContext.Session.GetString("USERLOGADO"), out int id))
            {
                Rating r = await _context.Ratings.FirstOrDefaultAsync(m => m.User_Id == id && m.Post_Id == post);
                if (r == null)
                {
                    Rating rNew = new Rating
                    {
                        User_Id = id,
                        Post_Id = post
                    };
                    _context.Ratings.Add(rNew);
                    return Json(new { success = true, redirectUrl = Url.Action("Main", "Aplication") }); // Retorna JSON com a URL de redirecionamento
                }
                else
                {
                    _context.Ratings.Remove(r);
                }
            }
            return Json(new { success = true, redirectUrl = Url.Action("Main", "Aplication") }); // Retorna JSON com a URL de redirecionamento
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
