using Inmobiliaria_2022.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria_2022.Controllers
{
    [Authorize]
    public class InquilinosController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly RepositorioInquilino repositorioInquilino;

        public InquilinosController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioInquilino = new RepositorioInquilino(configuration);
        }

        // GET: InquilinosController 
        public ActionResult Index()
        {
            try
            {
                var lista = repositorioInquilino.ObtenerTodos();
                ViewBag.Id = TempData["Id"];

                //if (TempData.ContainsKey("Mensaje"))
                //    ViewBag.Mensaje = TempData["Mensaje"];
                return View(lista);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // GET: InquilinosController/Details/5
        public ActionResult Details(int id)
        {          
            var entidad = repositorioInquilino.ObtenerPorId(id);
            return View(entidad);
        }

        // GET: InquilinosController/Create
        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // POST: InquilinosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inquilino i)
        {   
            try
            {
                int res = repositorioInquilino.Alta(i);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // GET: InquilinosController/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var entidad = repositorioInquilino.ObtenerPorId(id);
                return View(entidad);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // POST: InquilinosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            // Si en lugar de IFormCollection ponemos Propietario, el enlace de datos lo hace el sistema
            //try
            //{
            //    // TODO: Add update logic here
            //    repositorioInquilino.Modificacion(i);
            //    return RedirectToAction(nameof(Index));
            //}
            //catch
            //{
            //    ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
            //    var y = repositorioInquilino.ObtenerPorId(id);
            //    return View(y);
            //}
            Inquilino i = null;
            try
            {
                i = repositorioInquilino.ObtenerPorId(id);

                i.Nombre = collection["Nombre"];
                i.Apellido = collection["Apellido"];
                i.Dni = collection["Dni"];
                i.Telefono = collection["Telefono"];
                i.Email = collection["Email"];
                repositorioInquilino.Modificacion(i);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // GET: InquilinosController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            try
            {
                var entidad = repositorioInquilino.ObtenerPorId(id);
                return View(entidad);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // POST: InquilinosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Propietario entidad)
        {
            try
            {
                repositorioInquilino.Baja(id);
                //TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
