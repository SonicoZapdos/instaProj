using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using instaProj.Models;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Net;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace instaProj.Controllers
{
    public class PostsController : Controller
    {
        private readonly Contexto _context;

        public PostsController(Contexto context)
        {
            _context = context;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var contexto = _context.Posts.Include(p => p.User);
            return View(await contexto.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost([Bind("Id, Description, DataPub, Private, User_Id, User, ContLike, Archives")] Post post, List<IFormFile> archive)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(post.Description))
                {
                    post.User_Id = int.Parse(HttpContext.Session.GetString("USERLOGADO"));
                    post.DatePub = DateTime.Now;
                    post.Private = false;

                    _context.Add(post);
                    await _context.SaveChangesAsync();
                    if (archive != null)
                    {
                        await CreateArchive(post.Id, archive);
                    }

                    return RedirectToAction("Main", "Aplication");
                }
            }

            return View(post);
        }

        public async List<Post> ListPosts(int idUser)
        {
            List<Post> p = await _context.Posts.(m => m.Id == idUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArchive(int postId, List<IFormFile> arquivos )
        {
            foreach (var arquivo in arquivos)
            {
                if (arquivo != null && arquivo.Length < 10)
                {
                    // Processar imagem
                    string nomeArquivo = Path.GetFileName(arquivo.FileName);
                    string extensaoArquivo = Path.GetExtension(arquivo.FileName).ToLower();
                    string novoNome = geraNomeRandomizado(25) + extensaoArquivo;

                    // Diretórios de armazenamento
                    string caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", novoNome);
                    string caminhoMiniatura = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgMINI", novoNome);

                    // Salvar arquivo no servidor
                    using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
                    {
                        await arquivo.CopyToAsync(stream);
                    }

                    // Processar a imagem e criar miniatura
                    using (var image = Image.Load(caminhoArquivo))
                    {
                        int larguraDesejada = 100;
                        int alturaDesejada = 100;

                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(larguraDesejada, alturaDesejada),
                            Mode = ResizeMode.Max
                        }));

                        image.Save(caminhoMiniatura, new JpegEncoder());
                    }

                    var imagem = new Archive
                    {
                        Link = novoNome,
                        NameLocal = "/img/" + novoNome,
                        Post_Id = postId
                    };

                    // Grava dados no banco de dados
                    _context.Add(imagem);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Main", "Aplication");
        }

        public string VerificaExtensao(string nomeArquivo)
        {
            string extensaoArquivo = Path.GetExtension(nomeArquivo).ToLower();
            string[] validacaoLista = { ".gif", ".jpeg", ".jpg", ".png", ".mp4", ".mp3" };

            return validacaoLista.Contains(extensaoArquivo) ? extensaoArquivo : "none";
        }

        static string geraNomeRandomizado(int comprimento)
        {
            const string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new();
            return new string(Enumerable.Range(0, comprimento).Select(_ => caracteres[random.Next(caracteres.Length)]).ToArray());
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,ContLike,DatePub,Private,User_Id")] Post post)
        {
            if (ModelState.IsValid)
            {
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["User_Id"] = new SelectList(_context.Users, "Id", "Email", post.User_Id);
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["User_Id"] = new SelectList(_context.Users, "Id", "Email", post.User_Id);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,ContLike,DatePub,Private,User_Id")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["User_Id"] = new SelectList(_context.Users, "Id", "Email", post.User_Id);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'Contexto.Posts'  is null.");
            }
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
          return (_context.Posts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
