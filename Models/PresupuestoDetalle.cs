namespace Models
{
    public class PresupuestoDetalle
    {
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }


        public PresupuestoDetalle() {}

        public PresupuestoDetalle(Producto producto, int cantidad)
        {
            Producto = producto;
            Cantidad = cantidad;
        }
    }
}
