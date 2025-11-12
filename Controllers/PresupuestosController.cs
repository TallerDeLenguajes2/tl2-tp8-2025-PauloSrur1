using Microsoft.AspNetCore.Mvc;
using Models;
using Repositories;

namespace tl2_tp8_2025_PauloSrur1.Controllers
{
    public class PresupuestosController : Controller
    {
        private readonly PresupuestoRepository _presupuestoRepository;

        public PresupuestosController()
        {
            _presupuestoRepository = new PresupuestoRepository();
        }

        // GET: /Presupuestos
        [HttpGet]
        public IActionResult Index()
        {
            List<Presupuesto> presupuestos = _presupuestoRepository.Listar();
            return View(presupuestos);
        }
    }
}