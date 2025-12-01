using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using tl2_tp8_2025_PauloSrur1.Interfaces;
using Models;

namespace Repositories
{
    public class UsuarioRepository : IUserRepository
    {
        private readonly string _cadenaConexion;
        public UsuarioRepository(IConfiguration config)
        {
            _cadenaConexion = config.GetConnectionString("SQLite") ?? "Data Source=Tienda.db;";
        }

        public Usuario? GetUser(string username, string password)
        {
            using var conexion = new SqliteConnection(_cadenaConexion);
            conexion.Open();
            const string sql = @"SELECT Id, Nombre, User, Pass, Rol FROM Usuarios WHERE User = @Usuario AND Pass = @Contrasena";
            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.AddWithValue("@Usuario", username);
            comando.Parameters.AddWithValue("@Contrasena", password);
            using var reader = comando.ExecuteReader();
            if (reader.Read())
            {
                return new Usuario
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    User = reader.GetString(2),
                    Pass = reader.GetString(3),
                    Rol = reader.GetString(4)
                };
            }
            return null;
        }
    }
}