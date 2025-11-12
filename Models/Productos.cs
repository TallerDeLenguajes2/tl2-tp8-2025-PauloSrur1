namespace Models
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string Descripcion { get; set; }
        public int Precio { get; set; }

        public Producto() {}

        public Producto(int id, string descripcion, int precio)
        {
            IdProducto = id;
            Descripcion = descripcion;
            Precio = precio;
        }
    }
}
