using Inmobiliaria_2022.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Inmobiliaria_2022.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IConfiguration configuracion;
        private readonly RepositorioUsuario repositorioUsuario;
        public UsuariosController(IConfiguration configuration)
        {
            this.configuracion = configuration;
            repositorioUsuario = new RepositorioUsuario(configuration);
        }
        // GET: UsuariosController
        [Authorize(Policy = "Administrador")]
        public ActionResult Index()
        {
            //ViewBag.Roles = Usuario.ObtenerRoles();
            var lista = repositorioUsuario.ObtenerTodos();
            return View(lista);
        }

        // GET: UsuariosController/Details/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Details(int id)
        {
            var i = repositorioUsuario.ObtenerPorId(id);
            return View(i);
        }

        // GET: UsuariosController/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View();
        }

        
        [HttpPost]
        // POST: UsuariosController/Create
        //[Authorize(Policy = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Usuario u)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View();
            }
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: u.Clave,
                salt: System.Text.Encoding.ASCII.GetBytes(configuracion["Salt"]),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));
                u.Clave = hashed;
                u.Rol = User.IsInRole("SuperAdministrador") ? u.Rol : (int)enRoles.Empleado;
                var nbreRnd = Guid.NewGuid();//posible nombre aleatorio
                int res = repositorioUsuario.Alta(u);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View();
            }
        }

        // GET: UsuariosController/Edit/5
        [Authorize(Policy ="Administrador")]
        public ActionResult Edit(int id)
        {
            ViewData["Title"] = "Editar usuario";
            var u = repositorioUsuario.ObtenerPorId(id);
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View(u);
        }

        // POST: UsuariosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(int id, Usuario u)
        {
            var vista = "Edit";
            try
            {
                if (!User.IsInRole("Administrador"))
                {
                    vista = "Perfil";
                    var usuarioActual = repositorioUsuario.ObtenerPorEmail(User.Identity.Name);
                    if (usuarioActual.Id != id)//si no es admin, solo puede modificarse él mismo
                    {
                        repositorioUsuario.Modificacion(u);
                        return RedirectToAction(nameof(Index), "Home");
                    }
                    else
                    {
                        repositorioUsuario.Modificacion(u);
                    }
                }
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View(vista, u);
            }
        }

        // GET: UsuariosController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var res = repositorioUsuario.ObtenerPorId(id);
            return View(res);
        }

        // POST: UsuariosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Usuario entidad)
        {
            try
            {
                repositorioUsuario.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        [AllowAnonymous]
        // GET: Usuarios/Login/
        public ActionResult Login()
        {
            return View();
        }

        // POST: Usuarios/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginView login)//me llega un login
        {
            try
            {
                if (ModelState.IsValid)//chekeo q el login sea valido 
                {
                    //Convertir la clave de texto plano que tipio el usuario en un correspondiente hashed
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: login.Clave,
                    salt: System.Text.Encoding.ASCII.GetBytes(configuracion["Salt"]),//es un string ,obtine los byte
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));

                    var e = repositorioUsuario.ObtenerPorEmail(login.Email);//Obtiene el usuario por el email(el q el dice q es)

                    //no existia ese usuario con email o las claves no estan bien
                    if (e == null || e.Clave != hashed) //Cuando lo recupero reviso q la clave guardada en la base de datos sea igual q la q el puso                                                                               
                    {

                        ModelState.AddModelError("", "El email o clave no son correctos");
                        return View();
                    }
                    var claims = new List<Claim>
                    {
                     //new Claim(ClaimTypes.Name,e.UsuarioId),
                       new Claim(ClaimTypes.Name, e.Email),
                       new Claim("TipoDeUsuario", e.RolNombre),
                       new Claim("FullName", e.Nombre + " " + e.Apellido),
                       new Claim(ClaimTypes.Role, e.RolNombre),
                    };
                    //claimsIdentity lleva como paraemtro el listado de esas claim y el nombre de la autenticwcion
                    var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction(nameof(Index), "Home");
                }

                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }
        // GET: Usuarios/Logout
        public async Task<ActionResult> Logout()//Solo ingresan  los usuarios autenticados 
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
    
