using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models;
using Repositories;

namespace tl2_tp8_2025_PauloSrur1.Controllers
{
    public class PresupuestosController : Controller
    {
        private readonly PresupuestoRepository _presupuestoRepository;

        public PresupuestosController(IConfiguration config)
        {
            var cs = config.GetConnectionString("SQLite") ?? "Data Source=Tienda.db;";
            _presupuestoRepository = new PresupuestoRepository(cs);
        }

        // GET: /Presupuestos
        [HttpGet]
        public IActionResult Index()
        {
            List<Presupuesto> presupuestos = _presupuestoRepository.Listar();
            return View(presupuestos);
        }

        // GET: /Presupuestos/Details/{id}
        [HttpGet]
        public IActionResult Details(int id)
        {
            var presupuesto = _presupuestoRepository.ObtenerPorId(id);
            if (presupuesto == null)
            {
                return NotFound();
            }
            return View(presupuesto);
        }

        // GET: /Presupuestos/Create
        [HttpGet]
        public IActionResult Create()
        {
            var p = new Presupuesto { FechaCreacion = DateTime.Today };
            return View(p);
        }

        // POST: /Presupuestos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Presupuesto model)
        {
            if (string.IsNullOrWhiteSpace(model.NombreDestinatario))
            {
                ModelState.AddModelError(nameof(model.NombreDestinatario), "El destinatario es obligatorio");
            }
            if (!ModelState.IsValid) return View(model);

            _presupuestoRepository.Crear(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Presupuestos/Edit/{id}
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var presupuesto = _presupuestoRepository.ObtenerPorId(id);
            if (presupuesto == null) return NotFound();
            return View(presupuesto);
        }

        // POST: /Presupuestos/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Presupuesto model)
        {
            if (id != model.IdPresupuesto) return BadRequest();
            if (string.IsNullOrWhiteSpace(model.NombreDestinatario))
            {
                ModelState.AddModelError(nameof(model.NombreDestinatario), "El destinatario es obligatorio");
            }
            if (!ModelState.IsValid) return View(model);

            _presupuestoRepository.Modificar(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Presupuestos/Delete/{id}
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var presupuesto = _presupuestoRepository.ObtenerPorId(id);
            if (presupuesto == null) return NotFound();
            return View(presupuesto);
        }

        // POST: /Presupuestos/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int IdPresupuesto)
        {
            _presupuestoRepository.Eliminar(IdPresupuesto);
            return RedirectToAction(nameof(Index));
        }
    }
}


