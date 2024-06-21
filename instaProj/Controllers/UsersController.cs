using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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


        public IActionResult Maim()
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Password,Telefone,Url,Bio")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                WriteCookie(user.Id.ToString());

                Console.WriteLine("Entrou no IF");

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
                if (string.IsNullOrWhiteSpace(user.Telefone))
                {
                    ModelState.AddModelError("Telefone", "Campo Telefone não pode ser Vazio!");
                }

                return View(user); // Return to the form view with validation errors
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Name, string Password)
        {
            string usuario = Name;
            string senha = Password;

            var pessoa = await _context.Users.FirstOrDefaultAsync(m => m.Name == usuario);

            if (pessoa != null && pessoa.Password == senha)
            {
                
               // WriteCookie(pessoa.Id.ToString());

                Console.WriteLine("Entrou no IF");

                HttpContext.Session.SetString("USERLOGADO", pessoa.Id.ToString());

                return RedirectToAction("verifyLogin", "Users");
            }

            return RedirectToAction("verifyLogin", "Users");
        }

        public async Task<IActionResult> VerifyLogin()
        {
            string? userId = HttpContext.Session.GetString("USERLOGADO");

            if (!string.IsNullOrEmpty(userId))
            {
                // Tenta converter userId para um inteiro
                if (int.TryParse(userId, out int parsedUserId))
                {
                    User? pessoaLogada = await _context.Users.FirstOrDefaultAsync(m => m.Id == parsedUserId);

                    if (pessoaLogada != null)
                    {
                        string? nomeUsuarioLogado = pessoaLogada.Name;

                        bool cookieRecebido = HttpContext.Request.Cookies.TryGetValue("LOGADO", out string? valor);

                        if (cookieRecebido)
                        {
                            Console.WriteLine("\n\n:::::::: Valor do Cookie ::::::::    " + valor);
                        }
                        return RedirectToAction("Main", "Aplication");
                    }
                }
            }

            return RedirectToAction("Login", "Aplication");
        }

        public IActionResult WriteCookie(string value)
        {
            Response.Cookies.Append("LOGADO", value, new Microsoft.AspNetCore.Http.CookieOptions
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

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(int id, [Bind("Id,Name,Email,Password,Telefone,Url,Bio")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Main","Aplication", new { page = "MyPage"});
            }
            return View(user);
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

        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
