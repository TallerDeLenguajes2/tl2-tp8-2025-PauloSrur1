using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models;
using Repositories;

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
            return View(new Producto());
        }

        // POST: /Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Producto model)
        {
            if (string.IsNullOrWhiteSpace(model.Descripcion))
            {
                ModelState.AddModelError(nameof(model.Descripcion), "La descripción es obligatoria");
            }
            if (model.Precio <= 0)
            {
                ModelState.AddModelError(nameof(model.Precio), "El precio debe ser mayor a 0");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _productoRepository.Crear(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Productos/Edit/{id}
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var producto = _productoRepository.ObtenerPorId(id);
            if (producto == null) return NotFound();
            return View(producto);
        }

        // POST: /Productos/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Producto model)
        {
            if (id != model.IdProducto)
            {
                return BadRequest();
            }
            if (string.IsNullOrWhiteSpace(model.Descripcion))
            {
                ModelState.AddModelError(nameof(model.Descripcion), "La descripción es obligatoria");
            }
            if (model.Precio <= 0)
            {
                ModelState.AddModelError(nameof(model.Precio), "El precio debe ser mayor a 0");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

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
        public IActionResult DeleteConfirmed(int id)
        {
            _productoRepository.Eliminar(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
