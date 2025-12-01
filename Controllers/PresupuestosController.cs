using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models;
using tl2_tp8_2025_PauloSrur1.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using tl2_tp8_2025_PauloSrur1.Interfaces;

namespace tl2_tp8_2025_PauloSrur1.Controllers
{
    public class PresupuestosController : Controller
    {
        private readonly IPresupuestoRepository _presupuestoRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IAuthenticationService _authService;

        public PresupuestosController(IPresupuestoRepository presupuestoRepository,
                                      IProductoRepository productoRepository,
                                      IAuthenticationService authService)
        {
            _presupuestoRepository = presupuestoRepository;
            _productoRepository = productoRepository;
            _authService = authService;
        }

        // GET: /Presupuestos
        [HttpGet]
        public IActionResult Index()
        {
            if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");
            if (!(_authService.HasAccessLevel("Administrador") || _authService.HasAccessLevel("Cliente")))
                return RedirectToAction(nameof(AccesoDenegado));
            List<Presupuesto> presupuestos = _presupuestoRepository.Listar();
            return View(presupuestos);
        }

        // GET: /Presupuestos/Details/{id}
        [HttpGet]
        public IActionResult Details(int id)
        {
            if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");
            if (!(_authService.HasAccessLevel("Administrador") || _authService.HasAccessLevel("Cliente")))
                return RedirectToAction(nameof(AccesoDenegado));
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
            if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");
            if (!_authService.HasAccessLevel("Administrador")) return RedirectToAction(nameof(AccesoDenegado));
            var p = new PresupuestoViewModel { FechaCreacion = DateTime.Today };
            return View(p);
        }

        // POST: /Presupuestos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PresupuestoViewModel vm)
        {
            if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");
            if (!_authService.HasAccessLevel("Administrador")) return RedirectToAction(nameof(AccesoDenegado));
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
            if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");
            if (!_authService.HasAccessLevel("Administrador")) return RedirectToAction(nameof(AccesoDenegado));
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
            if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");
            if (!_authService.HasAccessLevel("Administrador")) return RedirectToAction(nameof(AccesoDenegado));
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
            if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");
            if (!_authService.HasAccessLevel("Administrador")) return RedirectToAction(nameof(AccesoDenegado));
            var presupuesto = _presupuestoRepository.ObtenerPorId(id);
            if (presupuesto == null) return NotFound();
            return View(presupuesto);
        }

        // POST: /Presupuestos/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int IdPresupuesto)
        {
            if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");
            if (!_authService.HasAccessLevel("Administrador")) return RedirectToAction(nameof(AccesoDenegado));
            _presupuestoRepository.Eliminar(IdPresupuesto);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AccesoDenegado()
        {
            return View();
        }

        // GET: /Presupuestos/AgregarProducto/{id}
        [HttpGet]
        public IActionResult AgregarProducto(int id)
        {
            // Solo administradores pueden modificar presupuestos
            if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");
            if (!_authService.HasAccessLevel("Administrador")) return RedirectToAction(nameof(AccesoDenegado));

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
            // Solo administradores pueden modificar presupuestos
            if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");
            if (!_authService.HasAccessLevel("Administrador")) return RedirectToAction(nameof(AccesoDenegado));

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


