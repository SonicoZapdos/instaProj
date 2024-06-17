﻿using System;
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
        public async Task<IActionResult> CreatePost([Bind("Id, Description, DataPub, Private, User_Id, User, ContLike")] Post post, List<IFormFile> Archives)
        {
            if (ModelState.IsValid)
            {
                post.User_Id = int.Parse(HttpContext.Session.GetString("USERLOGADO"));
                post.DatePub = DateTime.Now;
                post.Private = false;

                _context.Add(post);
                await _context.SaveChangesAsync();

                if (Archives != null && Archives.Count > 0)
                {
                    await CreateArchive(post.Id, Archives); // Chama o método para salvar os arquivos
                }

                return RedirectToAction("Main", "Aplication"); // Redireciona após o sucesso
            }

            return RedirectToAction("Main", "Aplication"); // Redireciona após o sucesso
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task CreateArchive(int postId, List<IFormFile> archives)
        {
            foreach (var archive in archives)
            {
                if (archive != null && archive.Length > 0)
                {
                    string extension = Path.GetExtension(archive.FileName).ToLower();
                    string filename = geraNomeRandomizado(25) + extension;
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", filename);


                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await archive.CopyToAsync(stream);
                    }

                    var archiveEntry = new Archive
                    {
                        Link = filename,
                        NameLocal = "/img/" + filename,
                        Post_Id = postId
                    };

                    _context.Add(archiveEntry);
                    await _context.SaveChangesAsync();
                }
            }
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