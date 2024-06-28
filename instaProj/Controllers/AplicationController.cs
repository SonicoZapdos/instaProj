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
                    List<Post> post = _context.Posts.Where(m => m.User_Id == id && m.Private != true).Include(m => m.User).OrderBy(m => m.DatePub).Reverse().ToList() ?? new List<Post>();
                    foreach (Post p in post) 
                    {
                        p.Archives = _context.Archives.Where(m => m.Post_Id == p.Id).ToList();

                        // Verifica se há uma avaliação (favorito) do usuário para o post
                        var ratingPostExists = _context.Ratings.Any(m => m.Post_Id == p.Id && m.User_Id == id);

                        // Atribui o valor de 'Rating' com base na existência da avaliação
                        p.Rating = ratingPostExists;

                        p.Comment = _context.Comments.Where(m => m.Post_Id == p.Id).Include(m => m.User).ToList() ?? new List<Comment>();

                        foreach(Comment c in p.Comment)
                        {
                            var ratingCommentExists = _context.Ratings.Any(m => m.Comment_Id == c.Id && m.User_Id == id);

                            c.Rating = ratingCommentExists;
                        }
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
                    post = _context.Posts.Where(m => m.User_Id != id && m.Private != true).Include(m => m.User).ToList() ?? new List<Post>();
                    foreach (Post p in post)
                    {
                        p.Archives = _context.Archives.Where(m => m.Post_Id == p.Id).ToList();

                        var ratingPostExists = _context.Ratings.Any(m => m.Post_Id == p.Id && m.User_Id == id);

                        p.Rating = ratingPostExists;

                        p.Comment = _context.Comments.Where(m => m.Post_Id == p.Id).Include(m => m.User).ToList() ?? new List<Comment>();

                        foreach (Comment c in p.Comment)
                        {
                            var ratingCommentExists = _context.Ratings.Any(m => m.Comment_Id == c.Id && m.User_Id == id);

                            c.Rating = ratingCommentExists;
                        }
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
            if (post != 0 && HttpContext.Session.GetString("USERLOGADO") != null && int.TryParse(HttpContext.Session.GetString("USERLOGADO"), out int userId))
            {
                // Verifica se já existe uma entrada de Rating para o usuário atual e o post específico
                Rating? rating = await _context.Ratings.FirstOrDefaultAsync(r => r.User_Id == userId && r.Post_Id == post);

                if (rating == null)
                {
                    // Se não existe, cria uma nova entrada de Rating
                    Rating r = new Rating
                    {
                        User_Id = userId,
                        Post_Id = post,
                        Comment_Id = 0,
                        SubComment_Id = 0
                    };
                    _context.Ratings.Add(r);
                }
                else
                {
                    // Se já existe uma entrada de Rating, remove-a
                    _context.Ratings.Remove(rating);
                }

                // Salva as mudanças no banco de dados apenas uma vez
                await _context.SaveChangesAsync();

                // Retorna o status da operação como JSON
                return Json(new { success = true });
            }

            // Retorna um erro caso não seja possível processar a requisição
            return BadRequest("Erro ao favoritar o post.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FavoriteComment(int post)
        {
            if (post != 0 && HttpContext.Session.GetString("USERLOGADO") != null && int.TryParse(HttpContext.Session.GetString("USERLOGADO"), out int userId))
            {
                // Verifica se já existe uma entrada de Rating para o usuário atual e o post específico
                Rating? rating = await _context.Ratings.FirstOrDefaultAsync(r => r.User_Id == userId && r.Comment_Id == post);

                if (rating == null)
                {
                    // Se não existe, cria uma nova entrada de Rating
                    Rating? r = new Rating
                    {
                        User_Id = userId,
                        Post_Id = 0,
                        Comment_Id = post,
                        SubComment_Id = 0
                    };
                    _context.Ratings.Add(r);
                }
                else
                {
                    // Se já existe uma entrada de Rating, remove-a
                    _context.Ratings.Remove(rating);
                }

                // Salva as mudanças no banco de dados apenas uma vez
                await _context.SaveChangesAsync();

                // Retorna o status da operação como JSON
                return Json(new { success = true });
            }

            // Retorna um erro caso não seja possível processar a requisição
            return BadRequest("Erro ao favoritar o post.");
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

        [HttpPost]
        public async Task<IActionResult> CreatePostComment(int postId, string description)
        {
            if (HttpContext.Session.GetString("USERLOGADO") != null && int.TryParse(HttpContext.Session.GetString("USERLOGADO"), out int userId))
            {
                var comment = new Comment
                {
                    Description = description,
                    Ocult = false,
                    User_Id = userId,
                    Post_Id = postId
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                var newComment = await _context.Comments
                    .Include(c => c.User)
                    .Where(c => c.Id == comment.Id)
                    .Select(c => new
                    {
                        c.Id,
                        c.Description,
                        User = new
                        {
                            c.User.Name,
                            PictureLocal = c.User.PictureLocal ?? "/images/ppic.png"
                        }
                    })
                    .FirstOrDefaultAsync();

                return Json(newComment);
            }

            return BadRequest("User not logged in");
        }



        [HttpPost]
        public async Task<IActionResult> CreatePostCommentMyPage(int postId, string description, string page)
        {
            if (HttpContext.Session.GetString("USERLOGADO") != null && int.TryParse(HttpContext.Session.GetString("USERLOGADO"), out int userId))
            {
                var comment = new Comment
                {
                    Description = description,
                    Ocult = false,
                    User_Id = userId,
                    Post_Id = postId
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    var response = new
                    {
                        Id = comment.Id,
                        Description = comment.Description,
                        User = new
                        {
                            Name = user.Name,
                            PictureLocal = user.PictureLocal
                        }
                    };

                    return Json(response);
                }
            }

            return BadRequest("Erro ao adicionar o comentário");
        }

    }
}
    

