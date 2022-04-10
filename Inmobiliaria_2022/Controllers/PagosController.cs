using Inmobiliaria_2022.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria_2022.Controllers
{
    //[Authorize]
    public class PagosController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly RepositorioPago repositorioPago;
        private readonly RepositorioContrato repositorioContrato;
        private readonly RepositorioInquilino repositorioInquilino;
        public PagosController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioContrato = new RepositorioContrato(configuration);
            repositorioPago = new RepositorioPago(configuration);
            repositorioInquilino = new RepositorioInquilino(configuration);
        }
   
        // GET: PagosController
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Listapago(string id)
        {
            try
            {
                var lista = repositorioPago.ObtenerPorContrato(id);
                return View(lista);
            }
            catch
            {
                return View();
            }
        }

        // GET: PagosController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PagosController/Create
        public ActionResult Create(int id)
        {
            ViewBag.Contrato = repositorioContrato.ObtenerPorId(id);
            ViewBag.Inquilino = repositorioInquilino.ObtenerInquilinoPorIdContrato(id);
            ViewBag.Pago = repositorioPago.ObtenerNumeroDePagoPorIdContrato(id);
            return View();
        }

        // POST: PagosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string Numero, DateTime Fecha, string Importe, int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Pago p = new Pago();
                    p.Numero = Numero;
                    p.Fecha = Fecha;
                    p.Importe = Importe;
                    p.ContratoId = id;

                    int res = repositorioPago.Alta(p);
                    string idAlquiler = id.ToString();
                    //return RedirectToAction(nameof(Listapago));
                    return RedirectToAction("Listapago", new { id });

                }
                else
                {
                    ViewBag.Alquileres = repositorioContrato.ObtenerTodos();
                    return View();
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: PagosController/Edit/5
        public ActionResult Edit(int id)
        {
            ViewBag.Pagos = repositorioPago.ObtenerTodos();
            var p = repositorioPago.ObtenerPorId(id);
            return View(p);
        }
        // POST: PagosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Pago p)
        {
            try
            {
                int res = repositorioPago.Modificacion(p);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Propietarios = repositorioPago.ObtenerTodos();
                var i = repositorioPago.ObtenerPorId(id);
                return View(i);
            }
        }

        // GET: PagosController/Delete/5
        public ActionResult Delete(int id)
        {
            var p = repositorioPago.ObtenerPorId(id);
            return View(p);
        }
        public ActionResult Delete(int id, Pago entidad)
        {
            try
            {
                repositorioPago.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch//(Exception ex)
            {
                return View();
            }
        }
    }
}
