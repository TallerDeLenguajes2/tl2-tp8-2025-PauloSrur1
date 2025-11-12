using Microsoft.AspNetCore.Mvc;
using Models;
using Repositories;

namespace tl2_tp8_2025_PauloSrur1.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ProductoRepository _productoRepository;

        public ProductosController()
        {
            _productoRepository = new ProductoRepository();
        }

        // GET: /Productos
        [HttpGet]
        public IActionResult Index()
        {
            List<Producto> productos = _productoRepository.Listar();
            return View(productos);
        }
    }
}