using System.Collections.Generic;
using Models;

namespace tl2_tp8_2025_PauloSrur1.Interfaces
{
    public interface IPresupuestoRepository
    {
        void Crear(Presupuesto presupuesto);
        List<Presupuesto> Listar();
        Presupuesto? ObtenerPorId(int id);
        void AgregarProducto(int idPresupuesto, int idProducto, int cantidad);
        void Modificar(Presupuesto presupuesto);
        bool Eliminar(int id);
    }
}