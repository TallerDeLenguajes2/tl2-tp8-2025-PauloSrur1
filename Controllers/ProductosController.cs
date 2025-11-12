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
    }
}