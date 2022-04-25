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
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly IConfiguration configuracion;
        private readonly IWebHostEnvironment environment;

        private readonly RepositorioUsuario repositorioUsuario;
        public UsuariosController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.configuracion = configuration;
            this.environment = environment;

            repositorioUsuario = new RepositorioUsuario(configuration);
        }
        // GET: UsuariosController
        //[Authorize(Policy = "Administrador")]
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
        //[Authorize(Policy = "Administrador")]
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
                //return RedirectToAction(nameof(Index)); saq 22/04/2022
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
                u.Rol = User.IsInRole("Administrador") ? u.Rol : (int)enRoles.Empleado;
                var nbreRnd = Guid.NewGuid();//posible nombre aleatorio
                int res = repositorioUsuario.Alta(u);
                if (u.AvatarFile != null && u.Id > 0) /*22/04*/
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    //Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
                    string fileName = "avatar_" + u.Id + Path.GetExtension(u.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    u.Avatar = Path.Combine("/Uploads", fileName);
                    // Esta operación guarda la foto en memoria en el ruta que necesitamos
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        u.AvatarFile.CopyTo(stream);
                    }
                    repositorioUsuario.Modificacion(u);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View();
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
        public ActionResult Delete(int id, Usuario usuario)
        {
            try
            {
                //if (System.IO.File.Exists(Path.Combine(environment.WebRootPath, "Uploads", "avatar" + id + Path.GetExtension(usuario.Avatar)));
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
                    salt: System.Text.Encoding.ASCII.GetBytes(configuracion["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));

                    var e = repositorioUsuario.ObtenerPorEmail(login.Email);


                    if (e == null || e.Clave != hashed)
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
                        new Claim("Avatar", e.Avatar), /*Img*/
                    };

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
        //[Route("salir", Name = "logout")]
        public async Task<ActionResult> Logout()//Solo ingresan  los usuarios autenticados 
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Authorize] /*22/04 (edita empleado) min 1:47*/
        public ActionResult Perfil()
        {
            ViewData["Title"] = "Mi perfil";
            var u = repositorioUsuario.ObtenerPorEmail(User.Identity.Name);
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View("Edit", u);
        }

        // GET: UsuariosController/Edit/5
        [Authorize(Policy = "Administrador")]
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
        public ActionResult Edit(int id, Usuario u)//Controlar contraseña min 1:21
        {
            var vista = nameof(Edit);
            try
            {
                if (!User.IsInRole("Administrador"))
                {
                    vista = nameof(Perfil);
                    var usuarioActual = repositorioUsuario.ObtenerPorEmail(User.Identity.Name);
                    if (usuarioActual.Id != id)//si no es admin, solo puede modificarse él mismo
                    {
                        repositorioUsuario.Modificacion(u);
                        return RedirectToAction(nameof(Index), "Home");

                    }

                }
                // TODO: Add update logic here
                else
                {
                    repositorioUsuario.Modificacion(u);
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(vista);

                //return RedirectToAction(nameof(Index)); 22/4
            }
            catch (Exception ex)
            {
                //ViewBag.Roles = Usuario.ObtenerRoles(); 22/4
                //return View(vista, u);
                throw;
            }

        }

        //-------------------------------------------------------
        // GET: UsuariosController/Cambiaravatar
        [Authorize]
        public ActionResult CambiarAvatar()
        {
            var u = repositorioUsuario.ObtenerPorEmail(User.Identity.Name);
            return View(u);
        }

        [HttpPost]
        public ActionResult CambiarAvatar(Usuario u)
        {
            try
            {
                if (u.AvatarFile != null && u.Id > 0) 
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    //Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
                    string fileName = "avatar_" + u.Id + Path.GetExtension(u.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    u.Avatar = Path.Combine("/Uploads", fileName);
                    // Esta operación guarda la foto en memoria en el ruta que necesitamos
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        u.AvatarFile.CopyTo(stream);
                    }
                    repositorioUsuario.CambiarAvatar(u);
                }
                return RedirectToAction(nameof(Index), "Home");
            }
            catch (Exception ex)
            {

                return View();
            }
        }


        [Authorize]
        public ActionResult CambiarClave()
        {

            return View();
        }

        [HttpPost]
        public ActionResult CambiarClave(CambioClaveView u)
        {
            try
            {

                string hashedVieja = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: u.ClaveVieja,
                salt: System.Text.Encoding.ASCII.GetBytes(configuracion["Salt"]),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));

                var us = repositorioUsuario.ObtenerPorEmail(User.Identity.Name);


                if (us.Clave != hashedVieja)
                {

                    ModelState.AddModelError("", "Clave no es correctos");
                    return View();
                }
                else
                {
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: u.ClaveNueva,
                    salt: System.Text.Encoding.ASCII.GetBytes(configuracion["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
                    u.ClaveNueva = hashed;

                    int res = repositorioUsuario.CambiarClave(us.Id, u);
                }

                return RedirectToAction(nameof(Index), "Home");
            }
            catch (Exception ex)
            {

                return View();
            }
        }

    }
}

