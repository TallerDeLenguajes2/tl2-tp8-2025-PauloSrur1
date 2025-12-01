using Microsoft.AspNetCore.Http;
using tl2_tp8_2025_PauloSrur1.Interfaces;

namespace tl2_tp8_2025_PauloSrur1.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool Login(string username, string password)
        {
            var ctx = _httpContextAccessor.HttpContext;
            var user = _userRepository.GetUser(username, password);
            if (user != null)
            {
                if (ctx == null) throw new InvalidOperationException("HttpContext no disponible");
                ctx.Session.SetString("IsAuthenticated", "true");
                ctx.Session.SetString("User", user.User);
                ctx.Session.SetString("UserNombre", user.Nombre);
                ctx.Session.SetString("Rol", user.Rol);
                return true;
            }
            return false;
        }

        public void Logout()
        {
            var ctx = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext no disponible");
            ctx.Session.Clear();
        }

        public bool IsAuthenticated()
        {
            var ctx = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext no disponible");
            return ctx.Session.GetString("IsAuthenticated") == "true";
        }

        public bool HasAccessLevel(string requiredAccessLevel)
        {
            var ctx = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext no disponible");
            return ctx.Session.GetString("Rol") == requiredAccessLevel;
        }
    }
}