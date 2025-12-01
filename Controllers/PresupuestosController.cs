using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models;
using Repositories;
using tl2_tp8_2025_PauloSrur1.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace tl2_tp8_2025_PauloSrur1.Controllers
{
    public class PresupuestosController : Controller
    {
        private readonly PresupuestoRepository _presupuestoRepository;
        private readonly ProductoRepository _productoRepository;

        public PresupuestosController(IConfiguration config)
        {
            var cs = config.GetConnectionString("SQLite") ?? "Data Source=Tienda.db;";
            _presupuestoRepository = new PresupuestoRepository(cs);
            _productoRepository = new ProductoRepository(cs);
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
            var p = new PresupuestoViewModel { FechaCreacion = DateTime.Today };
            return View(p);
        }

        // POST: /Presupuestos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PresupuestoViewModel vm)
        {
            if (vm.FechaCreacion.Date > DateTime.Today)
            {
                ModelState.AddModelError(nameof(vm.FechaCreacion), "La fecha no puede ser futura.");
            }
            if (!ModelState.IsValid) return View(vm);

            var model = new Presupuesto { NombreDestinatario = vm.NombreDestinatario ?? string.Empty, FechaCreacion = vm.FechaCreacion };
            _presupuestoRepository.Crear(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Presupuestos/Edit/{id}
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var presupuesto = _presupuestoRepository.ObtenerPorId(id);
            if (presupuesto == null) return NotFound();
            var vm = new PresupuestoViewModel { IdPresupuesto = presupuesto.IdPresupuesto, NombreDestinatario = presupuesto.NombreDestinatario, FechaCreacion = presupuesto.FechaCreacion };
            return View(vm);
        }

        // POST: /Presupuestos/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, PresupuestoViewModel vm)
        {
            if (id != vm.IdPresupuesto) return BadRequest();
            if (vm.FechaCreacion.Date > DateTime.Today)
            {
                ModelState.AddModelError(nameof(vm.FechaCreacion), "La fecha no puede ser futura.");
            }
            if (!ModelState.IsValid) return View(vm);

            var model = new Presupuesto { IdPresupuesto = vm.IdPresupuesto, NombreDestinatario = vm.NombreDestinatario ?? string.Empty, FechaCreacion = vm.FechaCreacion };
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

        // GET: /Presupuestos/AgregarProducto/{id}
        [HttpGet]
        public IActionResult AgregarProducto(int id)
        {
            var productos = _productoRepository.Listar();
            var vm = new AgregarProductoViewModel
            {
                IdPresupuesto = id,
ListaProductos = new SelectList(productos, nameof(Producto.IdProducto), nameof(Producto.Descripcion))
            };
            return View(vm);
        }

        // POST: /Presupuestos/AgregarProducto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AgregarProducto(AgregarProductoViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var productos = _productoRepository.Listar();
vm.ListaProductos = new SelectList(productos, nameof(Producto.IdProducto), nameof(Producto.Descripcion));
                return View(vm);
            }

            _presupuestoRepository.AgregarProducto(vm.IdPresupuesto, vm.IdProducto, vm.Cantidad);
            return RedirectToAction(nameof(Details), new { id = vm.IdPresupuesto });
        }
    }
}


