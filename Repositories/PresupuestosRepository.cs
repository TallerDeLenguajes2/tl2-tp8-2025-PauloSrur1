using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Models;

using tl2_tp8_2025_PauloSrur1.Interfaces;
namespace Repositories{

public class PresupuestoRepository : IPresupuestoRepository
    {
        private string cadenaConexion = "Data Source=Tienda.db;";

        // Esquema FIJO según tu base Tienda.db
        private string tablaPres = "PresupuestosDetalle";
        private string colPresId = "idPresupuesto";
        private string colPresNombre = "NombreDestinatario";
        private string colPresFecha = "FechaCreacion";

        private string tablaDet = "Detalle";
        private string colDetPresId = "idPresupuesto";
        private string colDetProdId = "idProducto";
        private string colDetCant = "cantidad";

        // Tabla de productos
        private string tablaProd = "Productos";
        private string colProdId = "id";
        private string colProdDesc = "descripcion";
        private string colProdPrecio = "precio";

        public PresupuestoRepository(string? connectionString = null)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                cadenaConexion = connectionString;
            }
            // Esquema fijo: no necesitamos autodetección
        }

        private void DetectSchema()
        {
            // Intencionalmente vacío: usamos el esquema fijo de arriba para tu DB
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
            string sqlDetalle = $@"SELECT pd.{colDetCant}, pr.{colProdId}, pr.{colProdDesc}, pr.{colProdPrecio}
                                  FROM {tablaDet} pd
                                  INNER JOIN {tablaProd} pr ON pr.{colProdId} = pd.{colDetProdId}
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

        // Modificar presupuesto existente
        public void Modificar(Presupuesto presupuesto)
        {
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();

            string sql = $"UPDATE {tablaPres} SET {colPresNombre} = @nombre, {colPresFecha} = @fecha WHERE {colPresId} = @id";
            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.AddWithValue("@nombre", presupuesto.NombreDestinatario);
            comando.Parameters.AddWithValue("@fecha", presupuesto.FechaCreacion);
            comando.Parameters.AddWithValue("@id", presupuesto.IdPresupuesto);
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
