using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MercDevs_ej2.Models;
using System.Diagnostics;

//Para guardar datos del usuario
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
//FIN Datos usuairo

namespace MercDevs_ej2.Controllers
{
    public class LoginController : Controller
    {
        private readonly MercydevsEjercicio2Context _context;
        public LoginController(MercydevsEjercicio2Context context) {
            _context = context;
        }

        //GET USUARIO
        [HttpGet]
        public IActionResult Ingresar()
        {
            //Para validar la utenticción en caso que la sesión siga activa y no muestre el login
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
            //Fin vldir utenticcion
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Ingresar(UsuarioLogin usuarioLogin)
        {
            //busca un solo usuario que coincida el correo y password ingresada por el login
            var usuario = await _context.Usuarios.
                FirstOrDefaultAsync(u =>
                    u.Correo == usuarioLogin.Correo &&
                    u.Password == usuarioLogin.Password);

            Console.WriteLine("---------- usuario-----");
            Console.WriteLine(usuario);

            if (usuario == null)
            {
                ViewData["Mensaje"] = "Nombre de usuario o contraseña no Coinciden";
                return View();
            }
            //Para datos usuario login
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim("Id", usuario.IdUsuario.ToString())
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    properties
                );
            //FIn Usuario Login
            return RedirectToAction("Index", "Home");


        }
        //profe no logro entender como encryptar el codigo :C , uso el tripledes pero no me pesca ,por que no hay metodo registrar propio 
    }
}
