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
                    ViewBag.Following = _context.Follows.Where(m => m.User_Id_Followed == id).Include(m => m.User_Following).ToList() ?? new List<Follow>();
                    ViewBag.Followed = _context.Follows.Where(m => m.User_Id_Following == id).Include(m => m.User_Followed).ToList() ?? new List<Follow>();
                    ViewBag.MainPage = page ?? "ForYou";
                    List<Post> post = _context.Posts.Where(m => m.User_Id == id).OrderBy(m => m.DatePub).Reverse().ToList() ?? new List<Post>();
                    foreach (Post p in post) 
                    {
                        p.Archives = _context.Archives.Where(m => m.Post_Id == p.Id).ToList();

                        // Verifica se há uma avaliação (favorito) do usuário para o post
                        var ratingExists = _context.Ratings.Any(m => m.Post_Id == p.Id && m.User_Id == id);

                        // Atribui o valor de 'Rating' com base na existência da avaliação
                        p.Rating = ratingExists;

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
        public async Task<IActionResult> Favorite(int post, string page)
        {
            if (post != 0 && HttpContext.Session.GetString("USERLOGADO") != null && int.TryParse(HttpContext.Session.GetString("USERLOGADO"), out int id))
            {
                // Verifica se já existe uma entrada de Rating para o usuário atual e o post específico
                Rating? r = await _context.Ratings.FirstOrDefaultAsync(m => m.User_Id == id && m.Post_Id == post) ?? null;

                if (r == new Rating() || r == null)
                {
                    // Se não existe, cria uma nova entrada de Rating
                    Rating rNew = new Rating
                    {
                        User_Id = id,
                        Post_Id = post,
                        Comment_Id = 0,
                        SubComment_Id = 0
                    };
                    _context.Ratings.Add(rNew);
                }
                else
                {
                    // Se já existe uma entrada de Rating, remove-a
                    _context.Ratings.Remove(r);
                }

                // Salva as mudanças no banco de dados
                await _context.SaveChangesAsync();
            }

            // Redireciona para a página principal após a operação
            return RedirectToAction("Main", "Aplication", new { page = page });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Follow(int followedId, int followingId)
        {
                var follow = new Follow
                {
                    User_Id_Following = followingId,
                    User_Id_Followed = followedId
                };
                if (!_context.Follows.Contains(follow))
                {
                    _context.Follows.Add(follow);
                    await _context.SaveChangesAsync();
                }
            return RedirectToAction("Main");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unfollow(int followedId, int followingId)
        {
                var follow = await _context.Follows.FirstOrDefaultAsync(f => f.User_Id_Following == followingId && f.User_Id_Followed == followedId);
                if (follow != null)
                {
                    _context.Follows.Remove(follow);
                    await _context.SaveChangesAsync();
            }
            return RedirectToAction("Main");
        }
    }
}
