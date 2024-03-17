using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pruebaimage2.Models;

namespace Pruebaimage2.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly Pruebaimage2DbContext _context;

        public UsuariosController(Pruebaimage2DbContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index(Usuario usuario)
        {
            var query = _context.Usuarios.AsQueryable();
            if (string.IsNullOrWhiteSpace(usuario.Nombre) == false)
            {
                query = query.Where(s => s.Nombre.Contains(usuario.Nombre));
            }
            if (string.IsNullOrWhiteSpace(usuario.Apellido) == false)
            {
                query = query.Where(s => s.Apellido.Contains(usuario.Apellido));
            }
            if (usuario.Estatus == 1 || usuario.Estatus == 2)
            {
                query = query.Where(s => s.Estatus == usuario.Estatus);
            }
            if (usuario.Take == 0)
                usuario.Take = 10;
            query = query.Take(usuario.Take);
            return query != null ? 
                          View(await query.ToListAsync()) :
                          Problem("Entity set 'Pruebaimage2DbContext.Usuarios'  is null.");
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login(string ReturnUrl)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([Bind("Email,Password")] Usuario usuario, string ReturnUrl)
        {
            usuario.Password = CalcularHashMD5(usuario.Password);
            var usuarioAut = await _context.Usuarios.FirstOrDefaultAsync(s => s.Email == usuario.Email && s.Password == usuario.Password && s.Estatus == 1);
            if (usuarioAut?.Id > 0 && usuarioAut.Email == usuario.Email)
            {
                var claims = new[] {
                    new Claim(ClaimTypes.Name, usuarioAut.Email),
                     new Claim(ClaimTypes.Role, usuarioAut.Rol),
                    new Claim("Id", usuarioAut.Id.ToString())
                    };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), new AuthenticationProperties { IsPersistent = true }); ;
                var result = User.Identity.IsAuthenticated;
                if (!string.IsNullOrWhiteSpace(ReturnUrl))
                    return Redirect(ReturnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }
            else
                ViewBag.Error = "Credenciales incorrectas";
            ViewBag.pReturnUrl = ReturnUrl;
            return View(usuario);
        }

        // GET: Usuarios/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Email,Password,Estatus,Rol,Comentario")] Usuario usuario)
        {
            usuario.Password = CalcularHashMD5(usuario.Password);
            _context.Add(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

            //return View(usuario);
        }

        // GET: Usuarios/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Email,Estatus,Rol,Comentario")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            try
            {
                var usuarioData = await _context.Usuarios.FirstOrDefaultAsync(s => s.Id == id);
                usuarioData.Nombre = usuario.Nombre;
                usuarioData.Apellido = usuario.Apellido;
                usuarioData.Email = usuario.Email;
                usuarioData.Estatus = usuario.Estatus;
                usuarioData.Rol = usuario.Rol;
                usuarioData.Comentario = usuario.Comentario;
                _context.Update(usuarioData);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(usuario.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));

            //return View(usuario);
        }

        // GET: Usuarios/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Usuarios == null)
            {
                return Problem("Entity set 'Pruebaimage2DbContext.Usuarios'  is null.");
            }
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool UsuarioExists(int id)
        {
            return (_context.Usuarios?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        private string CalcularHashMD5(string texto)
        {
            using (MD5 md5 = MD5.Create())
            {
                // Convierte la cadena de texto a bytes
                byte[] inputBytes = Encoding.UTF8.GetBytes(texto);

                // Calcula el hash MD5 de los bytes
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convierte el hash a una cadena hexadecimal
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}
