using Inmobiliaria_2022.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria_2022.Controllers
{
    [Authorize]
    public class ContratosController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly RepositorioContrato repositorioContrato;
        private readonly RepositorioInquilino repositorioInquilino;
        private readonly RepositorioInmueble repositorioInmueble;

        public ContratosController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioContrato = new RepositorioContrato(configuration);
            repositorioInquilino = new RepositorioInquilino(configuration);
            repositorioInmueble = new RepositorioInmueble(configuration);
        }
        // GET: ContratosController
        public ActionResult Index(int id,DateTime? fechaIni, DateTime? fechaFin)
        {   
            //String hoy = DateTime.Now.ToString("dd/MM/yyyy");
            var lista = repositorioContrato.ObtenerTodos();
            
            if (id == 0){
                lista = repositorioContrato.ObtenerTodos();
            }
            else if (id == 1){
                lista = repositorioContrato.ObtenerVigentes();
            }
            else if (id == 2){
                lista = repositorioContrato.ObtenerNoVigentes();
            }
            else if (id == 3){
                lista = repositorioContrato.ObtenerVigentesxFecha(fechaIni, fechaFin);
            }
            else if (id == 4){
                lista = repositorioContrato.ObtenerInmueblesDisponibles(fechaIni, fechaFin);
            }

            return View(lista);
        }


        // GET: ContratosController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var entidad = repositorioContrato.ObtenerPorId(id);
                return View(entidad);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // GET: ContratosController/Create
        public ActionResult Create()
        {
            ViewBag.Inmuebless = repositorioInmueble.ObtenerTodosDisponible();
            ViewBag.Inquilinoss = repositorioInquilino.ObtenerTodos();

            return View();
        }

        // POST: ContratosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contrato contrato)
        {
            try
            {
                repositorioContrato.Alta(contrato);
                repositorioInmueble.NoPublicado(contrato.InmuebleId);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ContratosController/Edit/5
        public ActionResult Edit(int id)
        {
            var al = repositorioContrato.ObtenerPorId(id);
            ViewBag.Inmuebless = repositorioInmueble.ObtenerTodosDisponible();
            ViewBag.Inquilinoss = repositorioInquilino.ObtenerTodos();
            return View(al);
        }

        // POST: ContratosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Contrato c)
        {
            try
            {
                repositorioContrato.Modificacion(c);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ContratosController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var p = repositorioContrato.ObtenerPorId(id);
            return View(p);
        }

        // POST: ContratosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Contrato c)
        {
            try
            {
                repositorioContrato.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        //public ActionResult ContratoVigentesxFecha(int vigencia, DateTime? fechaIni, DateTime? fechaFin)
        //{
        //    if (fechaIni != null || fechaFin != null)
        //    {
        //        var lista = repositorioContrato.ObtenerContratos(vigencia,fechaIni, fechaFin);
        //        return View(lista);
        //    }

        //    return View();
        //}
    }
}
