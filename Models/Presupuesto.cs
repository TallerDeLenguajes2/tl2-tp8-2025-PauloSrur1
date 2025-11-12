using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class Presupuesto
    {
        public int IdPresupuesto { get; set; }
        public string NombreDestinatario { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<PresupuestoDetalle> Detalle { get; set; } = new List<PresupuestoDetalle>();


        // Calcula el monto total (sin IVA)
        public decimal MontoPresupuesto()
        {
            return Detalle.Sum(d => d.Producto.Precio * d.Cantidad);
        }

        // Calcula el monto total con IVA del 21%
        public decimal MontoPresupuestoConIva()
        {
            decimal monto = MontoPresupuesto();
            return monto * 1.21m;
        }

        // Cuenta el total de productos (sumando las cantidades)
        public int CantidadProductos()
        {
            return Detalle.Sum(d => d.Cantidad);
        }
    }
}
