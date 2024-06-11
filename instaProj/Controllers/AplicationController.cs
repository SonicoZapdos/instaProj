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
                    User? pessoaLogada = _context.Users.FirstOrDefault(m => m.Id == int.Parse(userId));

                    if (pessoaLogada != null)
                    {
                        ViewBag.User = pessoaLogada;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForYou(Archive imagem,IFormFile NameLocal) /* [Bind("Id, Description, DataPub, Private, User_Id, User, ContLike")]Post post  */
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

                return RedirectToAction(nameof(Main));
            }
            if (arqRecebido == null && imagem.Link != "")
            {
                WebClient cli = new WebClient();

                var idVideo = imagem.Link.Substring(32);

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
