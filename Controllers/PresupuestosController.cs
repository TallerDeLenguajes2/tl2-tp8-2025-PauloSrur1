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
    }
}


