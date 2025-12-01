using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models;
using tl2_tp8_2025_PauloSrur1.ViewModels;
using tl2_tp8_2025_PauloSrur1.Interfaces;

namespace tl2_tp8_2025_PauloSrur1.Controllers
{
    public class ProductosController : Controller
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IAuthenticationService _authService;

        public ProductosController(IProductoRepository repo, IAuthenticationService authService)
        {
            _productoRepository = repo;
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var chk = CheckAdminPermissions(); if (chk != null) return chk;
            List<Producto> productos = _productoRepository.Listar();
            return View(productos);
        }

        // GET: /Productos/Create
        [HttpGet]
        public IActionResult Create()
        {
            var chk = CheckAdminPermissions(); if (chk != null) return chk;
            return View(new ProductoViewModel());
        }

        // POST: /Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductoViewModel vm)
        {
            var chk = CheckAdminPermissions(); if (chk != null) return chk;
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var model = new Producto { Descripcion = vm.Descripcion ?? string.Empty, Precio = vm.Precio };
            _productoRepository.Crear(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Productos/Edit/{id}
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var chk = CheckAdminPermissions(); if (chk != null) return chk;
            var producto = _productoRepository.ObtenerPorId(id);
            if (producto == null) return NotFound();
            var vm = new ProductoViewModel { IdProducto = producto.IdProducto, Descripcion = producto.Descripcion, Precio = producto.Precio };
            return View(vm);
        }

        // POST: /Productos/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ProductoViewModel vm)
        {
            var chk = CheckAdminPermissions(); if (chk != null) return chk;
            if (id != vm.IdProducto) return BadRequest();
            if (!ModelState.IsValid) return View(vm);

            var model = new Producto { IdProducto = vm.IdProducto, Descripcion = vm.Descripcion ?? string.Empty, Precio = vm.Precio };
            _productoRepository.Modificar(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Productos/Delete/{id}
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var chk = CheckAdminPermissions(); if (chk != null) return chk;
            var producto = _productoRepository.ObtenerPorId(id);
            if (producto == null) return NotFound();
            return View(producto);
        }

        // POST: /Productos/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int IdProducto)
        {
            var chk = CheckAdminPermissions(); if (chk != null) return chk;
            _productoRepository.Eliminar(IdProducto);
            return RedirectToAction(nameof(Index));
        }

        private IActionResult? CheckAdminPermissions()
        {
            if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");
            if (!_authService.HasAccessLevel("Administrador")) return RedirectToAction(nameof(AccesoDenegado));
            return null;
        }

        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}
