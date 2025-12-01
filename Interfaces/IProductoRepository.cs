using System.Collections.Generic;
using Models;

namespace tl2_tp8_2025_PauloSrur1.Interfaces
{
    public interface IProductoRepository
    {
        void Crear(Producto producto);
        void Modificar(Producto producto);
        List<Producto> Listar();
        Producto? ObtenerPorId(int id);
        bool Eliminar(int id);
    }
}
