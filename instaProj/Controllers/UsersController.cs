using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using instaProj.Models;

namespace instaProj.Controllers
{
    public class UsersController : Controller
    {
        private readonly Contexto _context;

        public UsersController(Contexto context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Main()
        {
            return View();
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return _context.Users != null ?
                View(await _context.Users.ToListAsync()) :
                Problem("Entity set 'Contexto.Users'  is null.");
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Password,Telefone,Url,Bio")] User user)
        {
            if (ModelState.IsValid)
            {
                string userUrl = "@" + user.Name.ToLower();
                user.Url = userUrl;

                _context.Add(user);
                await _context.SaveChangesAsync();
                WriteCookie(user.Id.ToString());

                HttpContext.Session.SetString("USERLOGADO", user.Id.ToString());
                return RedirectToAction("verifyLogin", "Users");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(user.Name))
                {
                    ModelState.AddModelError("Name", "Campo nome não pode ser Vazio!");
                }
                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    ModelState.AddModelError("Email", "Campo Email não pode ser Vazio!");
                }
                if (string.IsNullOrWhiteSpace(user.Password))
                {
                    ModelState.AddModelError("Password", "Campo Senha não pode ser Vazio!");
                }

                return View(user); // Return to the form view with validation errors
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Name, string Password)
        {
            var pessoa = await _context.Users.FirstOrDefaultAsync(m => m.Name == Name);

            if (pessoa != null && pessoa.Password == Password)
            {
                HttpContext.Session.SetString("USERLOGADO", pessoa.Id.ToString());
                return RedirectToAction("verifyLogin", "Users");
            }

            return RedirectToAction("verifyLogin", "Users");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("verifyLogin", "Users");
        }

        public async Task<IActionResult> verifyLogin()
        {
            string? userId = HttpContext.Session.GetString("USERLOGADO");

            if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int parsedUserId))
            {
                User? pessoaLogada = await _context.Users.FirstOrDefaultAsync(m => m.Id == parsedUserId);

                if (pessoaLogada != null)
                {
                    bool cookieRecebido = HttpContext.Request.Cookies.TryGetValue("LOGADO", out string? valor);

                    if (cookieRecebido)
                    {
                        Console.WriteLine("\n\n:::::::: Valor do Cookie ::::::::    " + valor);
                    }
                    return RedirectToAction("Main", "Aplication");
                }
            }

            return RedirectToAction("Login", "Aplication");
        }

        public IActionResult WriteCookie(string value)
        {
            Response.Cookies.Append("LOGADO", value, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.Now.AddDays(7)
            });
            return Ok("Cookie gravado com sucesso!");
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/UpdateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(int id, IFormFile PictureLocal)
        {
            var userToUpdate = await _context.Users.FindAsync(id);
            if (userToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<User>(userToUpdate, "", u => u.Name, u => u.Bio))
            {
                try
                {
                    if (PictureLocal != null && PictureLocal.Length > 0)
                    {
                        string picturePath = await UploadProfilePicture(PictureLocal);
                        userToUpdate.PictureLocal = picturePath;
                        Console.WriteLine(userToUpdate.PictureLocal);
                        Console.WriteLine(picturePath);

                    }
                    else
                    {
                        userToUpdate.PictureLocal = "/imgPerfil/" + userToUpdate.PictureLocal;
                        Console.WriteLine(userToUpdate.PictureLocal);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Main", "Aplication", new { page = "MyPage" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(userToUpdate.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Não foi possível salvar as alterações. Tente novamente.");
            }

            return RedirectToAction("Main", "Aplication", new { page = "MyPage" });
        }


        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'Contexto.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<string?> UploadProfilePicture(IFormFile profilePicture)
        {
            if (profilePicture != null && profilePicture.Length > 0)
            {
                string extension = Path.GetExtension(profilePicture.FileName).ToLower();
                string filename = geraNomeRandomizado(25) + extension;

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgPerfil")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgPerfil"));
                }
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgPerfil", filename);

                using (var stream = new FileStream(uploadPath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(stream);
                }

                return "/imgPerfil/" + filename;
            }

            return null;
        }

        static string geraNomeRandomizado(int comprimento)
        {
            const string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new();
            return new string(Enumerable.Range(0, comprimento).Select(_ => caracteres[random.Next(caracteres.Length)]).ToArray());
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
