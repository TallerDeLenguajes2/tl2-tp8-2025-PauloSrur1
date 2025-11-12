using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Models;

namespace Repositories{

public class PresupuestoRepository
    {
        private string cadenaConexion = "Data Source=Tienda.db;";

        // Nombres dinámicos de tablas/columnas según el esquema real
        private string tablaPres = "Presupuestos";
        private string colPresId = "idPresupuesto"; // fallback: id
        private string colPresNombre = "nombreDestinatario"; // fallback: NombreDestinatario
        private string colPresFecha = "fechaCreacion"; // fallback: FechaCreacion

        private string tablaDet = "PresupuestoDetalle"; // posibles: PresupuestosDetalle, presupuestosDetalle
        private string colDetPresId = "idPresupuesto";
        private string colDetProdId = "idProducto";
        private string colDetCant = "cantidad";

        public PresupuestoRepository()
        {
            DetectSchema();
        }

        private void DetectSchema()
        {
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();

            // Detectar nombres reales de tablas en sqlite_master
            string findTable(string desired)
            {
                using var cmd = new SqliteCommand("SELECT name FROM sqlite_master WHERE type='table'", conexion);
                using var r = cmd.ExecuteReader();
                var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                while (r.Read()) names.Add(r.GetString(0));
                if (names.Contains(desired)) return desired;
                // Variantes comunes
                foreach (var opt in new[]{ "presupuestos", "Presupuestos", "Presupuesto", "PresupuestoDetalle", "PresupuestosDetalle", "presupuestosDetalle" })
                {
                    if (names.Contains(opt)) return opt;
                }
                return desired; // por defecto
            }

            // Intentar preservar las deseadas pero aceptar variantes existentes
            var pres = findTable(tablaPres);
            if (!pres.Equals(tablaDet, StringComparison.OrdinalIgnoreCase)) tablaPres = pres;

            var det = findTable(tablaDet);
            tablaDet = det;

            // Detectar columnas con PRAGMA table_info
            HashSet<string> colsDe(string tabla)
            {
                using var cmd = new SqliteCommand($"PRAGMA table_info({tabla});", conexion);
                using var rdr = cmd.ExecuteReader();
                var cols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                while (rdr.Read()) cols.Add(rdr.GetString(1));
                return cols;
            }

            var presCols = colsDe(tablaPres);
            if (!presCols.Contains(colPresId) && presCols.Contains("id")) colPresId = "id";
            if (!presCols.Contains(colPresNombre) && presCols.Contains("NombreDestinatario")) colPresNombre = "NombreDestinatario";
            if (!presCols.Contains(colPresFecha) && presCols.Contains("FechaCreacion")) colPresFecha = "FechaCreacion";

            var detCols = colsDe(tablaDet);
            if (!detCols.Contains(colDetPresId) && detCols.Contains("IdPresupuesto")) colDetPresId = "IdPresupuesto";
            if (!detCols.Contains(colDetProdId))
            {
                if (detCols.Contains("IdProducto")) colDetProdId = "IdProducto";
                else if (detCols.Contains("productoId")) colDetProdId = "productoId";
                else if (detCols.Contains("id")) colDetProdId = "id"; // último recurso
            }
            if (!detCols.Contains(colDetCant) && detCols.Contains("Cantidad")) colDetCant = "Cantidad";
        }

        // Crear nuevo presupuesto
        public void Crear(Presupuesto presupuesto)
        {
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();

            string sql = $"INSERT INTO {tablaPres} ({colPresNombre}, {colPresFecha}) VALUES (@nombre, @fecha)";
            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.AddWithValue("@nombre", presupuesto.NombreDestinatario);
            comando.Parameters.AddWithValue("@fecha", presupuesto.FechaCreacion);
            comando.ExecuteNonQuery();
        }

        // Listar todos los presupuestos
        public List<Presupuesto> Listar()
        {
            var lista = new List<Presupuesto>();
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();

            string sql = $"SELECT {colPresId}, {colPresNombre}, {colPresFecha} FROM {tablaPres}";
            using var comando = new SqliteCommand(sql, conexion);
            using var lector = comando.ExecuteReader();

            while (lector.Read())
            {
                var p = new Presupuesto
                {
                    IdPresupuesto = lector.GetInt32(0),
                    NombreDestinatario = lector.GetString(1),
                    FechaCreacion = DateTime.Parse(lector.GetString(2))
                };
                lista.Add(p);
            }

            return lista;
        }

        // Obtener presupuesto con sus productos
        public Presupuesto? ObtenerPorId(int id)
        {
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();

            // Datos del presupuesto
            string sqlPresupuesto = $"SELECT {colPresId}, {colPresNombre}, {colPresFecha} FROM {tablaPres} WHERE {colPresId} = @id";
            using var cmdPres = new SqliteCommand(sqlPresupuesto, conexion);
            cmdPres.Parameters.AddWithValue("@id", id);
            using var lector = cmdPres.ExecuteReader();

            if (!lector.Read()) return null;

            var presupuesto = new Presupuesto
            {
                IdPresupuesto = lector.GetInt32(0),
                NombreDestinatario = lector.GetString(1),
                FechaCreacion = DateTime.Parse(lector.GetString(2))
            };

            lector.Close();

            // Cargar los detalles (JOIN con Productos)
            string sqlDetalle = $@"SELECT pd.{colDetCant}, pr.id, pr.descripcion, pr.precio
                                  FROM {tablaDet} pd
                                  INNER JOIN Productos pr ON pr.id = pd.{colDetProdId}
                                  WHERE pd.{colDetPresId} = @id";

            using var cmdDet = new SqliteCommand(sqlDetalle, conexion);
            cmdDet.Parameters.AddWithValue("@id", id);
            using var lectorDet = cmdDet.ExecuteReader();

            while (lectorDet.Read())
            {
                var detalle = new PresupuestoDetalle
                {
                    Producto = new Producto
                    {
                        IdProducto = lectorDet.GetInt32(1),
                        Descripcion = lectorDet.GetString(2),
                        Precio = lectorDet.GetInt32(3)
                    },
                    Cantidad = lectorDet.GetInt32(0)
                };
                presupuesto.Detalle.Add(detalle);
            }

            return presupuesto;
        }

        // Agregar un producto a un presupuesto
        public void AgregarProducto(int idPresupuesto, int idProducto, int cantidad)
        {
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();

            string sql = $"INSERT INTO {tablaDet} ({colDetPresId}, {colDetProdId}, {colDetCant}) VALUES (@pres, @prod, @cant)";
            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.AddWithValue("@pres", idPresupuesto);
            comando.Parameters.AddWithValue("@prod", idProducto);
            comando.Parameters.AddWithValue("@cant", cantidad);
            comando.ExecuteNonQuery();
        }

        // Eliminar presupuesto
        public bool Eliminar(int id)
        {
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();

            // Eliminar detalles primero
            string sqlDetalle = $"DELETE FROM {tablaDet} WHERE {colDetPresId} = @id";
            using var cmdDetalle = new SqliteCommand(sqlDetalle, conexion);
            cmdDetalle.Parameters.AddWithValue("@id", id);
            cmdDetalle.ExecuteNonQuery();

            // Luego el presupuesto
            string sqlPres = $"DELETE FROM {tablaPres} WHERE {colPresId} = @id";
            using var cmdPres = new SqliteCommand(sqlPres, conexion);
            cmdPres.Parameters.AddWithValue("@id", id);
            int filas = cmdPres.ExecuteNonQuery();

            return filas > 0;
        }
    }


}
