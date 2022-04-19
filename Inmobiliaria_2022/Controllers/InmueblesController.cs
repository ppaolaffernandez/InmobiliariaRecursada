using Inmobiliaria_2022.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inmobiliaria_2022.Controllers
{
    [Authorize]
    public class InmueblesController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly RepositorioPropietario repositorioPropietario;
        private readonly RepositorioInmueble repositorioInmueble;
        public InmueblesController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioPropietario = new RepositorioPropietario(configuration);
            repositorioInmueble = new RepositorioInmueble(configuration);
        }
        // GET: InmueblesController
        public ActionResult Index()
        {
            var lista = repositorioInmueble.ObtenerTodos();
            //if (TempData.ContainsKey("Id"))
            //    ViewBag.Id = TempData["Id"];
            //if (TempData.ContainsKey("Mensaje"))
            //    ViewBag.Mensaje = TempData["Mensaje"];
            return View(lista);
        }

        //public ActionResult PorPropietario(int id)
        //{
        //    var lista = repositorioInmueble.ObtenerTodos();//repositorio.ObtenerPorPropietario(id);
        //    //if (TempData.ContainsKey("Id"))
        //    //    ViewBag.Id = TempData["Id"];
        //    //if (TempData.ContainsKey("Mensaje"))
        //    //    ViewBag.Mensaje = TempData["Mensaje"];
        //    //ViewBag.Id = id;
        //    //ViewBag.Propietario = repoPropietario.
        //    return View("Index", lista);
        //}

        // GET: InmueblesController/Details/5
        public ActionResult Details(int id)
        {
            var entidad = repositorioInmueble.ObtenerPorId(id);
            return View(entidad);
        }

        // GET: InmueblesController/Create
        public ActionResult Create()
        {
            //ViewBag.TipoNombre = new SelectList(Inmueble.ObtenerTiposIDictionary().Select(x => new { Value = x.Key, Text = x.Value.Replace("_", " ") }), "Value", "Text");
            ViewBag.TipoNombre = Inmueble.ObtenerTiposIDictionary();
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
            return View();
        }

        // POST: InmueblesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inmueble entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repositorioInmueble.Alta(entidad);
                    TempData["Id"] = entidad.Id;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
                    return View(entidad);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        // GET: InmueblesController/Edit/5
        public ActionResult Edit(int id)
        {
            var entidad = repositorioInmueble.ObtenerPorId(id);
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
            //if (TempData.ContainsKey("Mensaje"))
            //    ViewBag.Mensaje = TempData["Mensaje"];
            //if (TempData.ContainsKey("Error"))
            //    ViewBag.Error = TempData["Error"];
            return View(entidad);
        }

        // POST: InmueblesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inmueble entidad)
        {
            try
            {
                entidad.Id = id;
                repositorioInmueble.Modificacion(entidad);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        // GET: InmueblesController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var entidad = repositorioInmueble.ObtenerPorId(id);
            //if (TempData.ContainsKey("Mensaje"))
            //    ViewBag.Mensaje = TempData["Mensaje"];
            //if (TempData.ContainsKey("Error"))
            //    ViewBag.Error = TempData["Error"];
            return View(entidad);
        }

        // POST: InmueblesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Inmueble entidad)
        {
            try
            {
                repositorioInmueble.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }
    }
}
