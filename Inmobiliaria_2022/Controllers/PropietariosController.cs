using Inmobiliaria_2022.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria_2022.Controllers
{
    [Authorize]
    public class PropietariosController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly RepositorioPropietario repositorioPropietario;
        private readonly RepositorioInmueble repositorioInmueble;

        public PropietariosController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioPropietario = new RepositorioPropietario(configuration);
            repositorioInmueble = new RepositorioInmueble(configuration);
        }

        // GET: PropietariosController
        public ActionResult Index()
        {
            var Lista = repositorioPropietario.ObtenerTodos();
            return View(Lista);
        }

        // GET: PropietariosController/Details/5
        public ActionResult Details(int id)
        {
            var i = repositorioPropietario.ObtenerPorId(id);
            return View(i); ;
        }

        // GET: PropietariosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PropietariosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Propietario p)
        {
            try
            {
                int res = repositorioPropietario.Alta(p);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // GET: PropietariosController/Edit/5
        public ActionResult Edit(int id)
        {

            try
            {
                var p = repositorioPropietario.ObtenerPorId(id);
                return View(p);//pasa el modelo a la vista
            }
            catch (Exception ex)
            {//poner breakpoints para detectar errores
                throw;
            }
        }

        // POST: PropietariosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection c)
        {
            Propietario p = null;
            try
            {
                p = repositorioPropietario.ObtenerPorId(id);
                p.Nombre = c["Nombre"];
                p.Apellido = c["Apellido"];
                p.Dni = c["Dni"];
                p.Email = c["Email"];
                p.Telefono = c["Telefono"];
                p.Clave = c["Clave"];
                repositorioPropietario.Modificacion(p);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {//poner breakpoints para detectar errores
                throw;
            }
            //try
            //{
            //    repositorioPropietario.Modificacion(p);
            //    return RedirectToAction(nameof(Index));

            //}
            //catch
            //{
            //    ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
            //    var a = repositorioPropietario.ObtenerPorId(id);

            //    return View(a);
            //}
        }

        // GET: PropietariosController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {

            try
            {
                var entidad = repositorioPropietario.ObtenerPorId(id);
                return View(entidad);
            }
            catch (Exception ex)
            {//poner breakpoints para detectar errores
                throw;
            };
        }

        // POST: PropietariosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Propietario entidad)
        {
            try
            {
                repositorioPropietario.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {//poner breakpoints para detectar errores
                throw;
            }
        }
        public ActionResult VerInmueble(string dni)//recibe 
        {
            try
            {
                var lista = repositorioInmueble.ObtenerInmueblePorDni(dni);
                return View(lista);
            }
            catch
            {
                return View();
            }
        }
    }
}
