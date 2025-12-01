using Models;

namespace tl2_tp8_2025_PauloSrur1.Interfaces
{
    public interface IUserRepository
    {
        Usuario? GetUser(string username, string password);
    }
}