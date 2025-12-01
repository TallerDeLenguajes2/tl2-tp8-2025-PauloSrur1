namespace Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Pass { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty; // "Administrador" o "Cliente"
    }
}