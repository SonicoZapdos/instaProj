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
    public class ArchivesController : Controller
    {
        private readonly Contexto _context;

        public ArchivesController(Contexto context)
        {
            _context = context;
        }

        // GET: Archives
        public async Task<IActionResult> Index()
        {
            var contexto = _context.Archives.Include(a => a.Post);
            return View(await contexto.ToListAsync());
        }

        // GET: Archives/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Archives == null)
            {
                return NotFound();
            }

            var archive = await _context.Archives
                .Include(a => a.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (archive == null)
            {
                return NotFound();
            }

            return View(archive);
        }

        // GET: Archives/Create
        public IActionResult Create()
        {
            ViewData["Post_Id"] = new SelectList(_context.Posts, "Id", "Id");
            return View();
        }

        // POST: Archives/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Link,NameLocal,Type,Post_Id")] Archive archive)
        {
            if (ModelState.IsValid)
            {
                _context.Add(archive);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Post_Id"] = new SelectList(_context.Posts, "Id", "Id", archive.Post_Id);
            return View(archive);
        }

        // GET: Archives/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Archives == null)
            {
                return NotFound();
            }

            var archive = await _context.Archives.FindAsync(id);
            if (archive == null)
            {
                return NotFound();
            }
            ViewData["Post_Id"] = new SelectList(_context.Posts, "Id", "Id", archive.Post_Id);
            return View(archive);
        }

        // POST: Archives/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Link,NameLocal,Type,Post_Id")] Archive archive)
        {
            if (id != archive.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(archive);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArchiveExists(archive.Id))
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
            ViewData["Post_Id"] = new SelectList(_context.Posts, "Id", "Id", archive.Post_Id);
            return View(archive);
        }

        // GET: Archives/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Archives == null)
            {
                return NotFound();
            }

            var archive = await _context.Archives
                .Include(a => a.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (archive == null)
            {
                return NotFound();
            }

            return View(archive);
        }

        // POST: Archives/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Archives == null)
            {
                return Problem("Entity set 'Contexto.Archives'  is null.");
            }
            var archive = await _context.Archives.FindAsync(id);
            if (archive != null)
            {
                _context.Archives.Remove(archive);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArchiveExists(int id)
        {
          return (_context.Archives?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public IActionResult ForYou()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForYou(Archive imagem, IFormFile NameLocal) /* [Bind("Id, Description, DataPub, Private, User_Id, User, ContLike")]Post post  */
        {
            var arqRecebido = Request.Form.Files["NameLocal"];

            if (arqRecebido != null && arqRecebido.Length > 0)
            {
                // Processar imagem
                string nomeArquivo = Path.GetFileName(arqRecebido.FileName);
                string extensaoArquivo = Path.GetExtension(arqRecebido.FileName).ToLower();
                string novoNome = geraNomeRandomizado(25) + extensaoArquivo;

                // Diretórios de armazenamento
                string caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", novoNome);
                string caminhoMiniatura = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgMINI", novoNome);

                // Salvar arquivo no servidor
                using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
                {
                    await arqRecebido.CopyToAsync(stream);
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

                imagem.NameLocal = novoNome;
                imagem.Link = "/img/" + novoNome;

                // Grava dados no banco de dados
                _context.Add(imagem);
                await _context.SaveChangesAsync();
                return RedirectToAction("verifyLogin", "Users");

            }
            if (arqRecebido == null && imagem.Link != "")
            {
                WebClient cli = new();

                var idVideo = (imagem.Link ?? "").Substring(32);

                var imgBytes = cli.DownloadData($"http://img.youtube.com/vi/{idVideo}/0.jpg");

                string fileName = geraNomeRandomizado(25);

                System.IO.File.WriteAllBytes(@"wwwroot/imgMINI/" + fileName + ".jpg", imgBytes);

                imagem.NameLocal = fileName + ".jpg";
                imagem.Link = fileName + ".jpg";

                _context.Add(imagem);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            // Se ocorrer um erro, retornar a view com o modelo atual
            return View(imagem);
        }

        public string VerificaExtensao(string nomeArquivo)
        {
            string extensaoArquivo = Path.GetExtension(nomeArquivo).ToLower();
            string[] validacaoLista = { ".gif", ".jpeg", ".jpg", ".png", ".mp4", ".mp3" };

            foreach (string extensao in validacaoLista)
            {
                if (extensao == extensaoArquivo)
                    return extensao;
            }

            return "none";
        }

        static string geraNomeRandomizado(int comprimento)
        {
            string[] caracteres = {
                "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m",
                "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
                "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
            };

            Random random = new Random();
            string nomeRandomizado = "";

            for (int i = 0; i < comprimento; i++)
            {
                int indice = random.Next(caracteres.Length);
                nomeRandomizado += caracteres[indice];
            }

            return nomeRandomizado;
        }

    }
}
