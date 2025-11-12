using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Models;

namespace Repositories
{
    public class ProductoRepository
    {
        private string cadenaConexion = "Data Source=Tienda.db;";

        // Configuración dinámica de nombres de tabla/columnas según el esquema real de la DB
        private readonly string tabla = "Productos";
        private string colId = "idProducto";
        private string colDesc = "descripcion";
        private string colPrecio = "precio";

        public ProductoRepository()
        {
            DetectColumns();
        }

        private void DetectColumns()
        {
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();
            using var cmd = new SqliteCommand("PRAGMA table_info(Productos);", conexion);
            using var reader = cmd.ExecuteReader();
            var cols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            while (reader.Read())
            {
                // PRAGMA table_info: [cid, name, type, notnull, dflt_value, pk]
                cols.Add(reader.GetString(1)); // name
            }
            if (!cols.Contains(colId))
            {
                if (cols.Contains("id")) colId = "id";
            }
            if (!cols.Contains(colDesc))
            {
                if (cols.Contains("Descripcion")) colDesc = "Descripcion";
                else if (cols.Contains("nombre")) colDesc = "nombre";
            }
            if (!cols.Contains(colPrecio))
            {
                if (cols.Contains("Precio")) colPrecio = "Precio";
                else if (cols.Contains("precioUnitario")) colPrecio = "precioUnitario";
            }
        }

        //logica para crear un nuevo producto 
        public void Crear(Producto producto)
        {
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();

            string sql = $"INSERT INTO {tabla} ({colDesc}, {colPrecio}) VALUES (@desc, @precio)";
            using var comando = new SqliteCommand(sql,conexion);
            comando.Parameters.AddWithValue("@desc", producto.Descripcion);
            comando.Parameters.AddWithValue("@precio", producto.Precio);
            comando.ExecuteNonQuery();
        }

        //logica para modificar un producto existente
        public void Modificar(Producto producto)
        {
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();

            string sql = $"UPDATE {tabla} SET {colDesc} = @desc, {colPrecio} = @precio WHERE {colId} = @id";
            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.AddWithValue("@desc", producto.Descripcion);
            comando.Parameters.AddWithValue("@precio", producto.Precio);
            comando.Parameters.AddWithValue("@id", producto.IdProducto);
            comando.ExecuteNonQuery();
        }

        //logica para listar todos los productos
        public List<Producto> Listar()
        {
            var lista = new List<Producto>();
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();

            string sql = $"SELECT {colId}, {colDesc}, {colPrecio} FROM {tabla}";
            using var comando = new SqliteCommand(sql, conexion);
            using var lector = comando.ExecuteReader();

            while (lector.Read())
            {
                var prod = new Producto
                {
                    IdProducto = lector.GetInt32(0),
                    Descripcion = lector.GetString(1),
                    Precio = lector.GetInt32(2)
                };
                lista.Add(prod);
            }

            return lista;
        }

        //logica para obtener un producto por ID
        public Producto? ObtenerPorId(int id)
        {
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();

            string sql = $"SELECT {colId}, {colDesc}, {colPrecio} FROM {tabla} WHERE {colId} = @id";
            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.AddWithValue("@id", id);

            using var lector = comando.ExecuteReader();
            if (lector.Read())
            {
                return new Producto
                {
                    IdProducto = lector.GetInt32(0),
                    Descripcion = lector.GetString(1),
                    Precio = lector.GetInt32(2)
                };
            }
            return null;
        }

        //logica para eliminar producto por ID
        public bool Eliminar(int id)
        {
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();

            string sql = $"DELETE FROM {tabla} WHERE {colId} = @id";
            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.AddWithValue("@id", id);

            int filasAfectadas = comando.ExecuteNonQuery();
            return filasAfectadas > 0;
        }







    }
}