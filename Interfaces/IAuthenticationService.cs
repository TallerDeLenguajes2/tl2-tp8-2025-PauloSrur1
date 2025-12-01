namespace tl2_tp8_2025_PauloSrur1.Interfaces
{
    public interface IAuthenticationService
    {
        bool Login(string username, string password);
        void Logout();
        bool IsAuthenticated();
        bool HasAccessLevel(string requiredAccessLevel);
    }
}