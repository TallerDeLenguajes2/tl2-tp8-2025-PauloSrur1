using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models;
using Repositories;
using tl2_tp8_2025_PauloSrur1.ViewModels;

namespace tl2_tp8_2025_PauloSrur1.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ProductoRepository _productoRepository;

        public ProductosController(IConfiguration config)
        {
            var cs = config.GetConnectionString("SQLite") ?? "Data Source=Tienda.db;";
            _productoRepository = new ProductoRepository(cs);
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Producto> productos = _productoRepository.Listar();
            return View(productos);
        }

        // GET: /Productos/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new ProductoViewModel());
        }

        // POST: /Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductoViewModel vm)
        {
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
            var producto = _productoRepository.ObtenerPorId(id);
            if (producto == null) return NotFound();
            return View(producto);
        }

        // POST: /Productos/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int IdProducto)
        {
            _productoRepository.Eliminar(IdProducto);
            return RedirectToAction(nameof(Index));
        }
    }
}
