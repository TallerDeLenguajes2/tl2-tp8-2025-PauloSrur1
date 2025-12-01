using Microsoft.AspNetCore.Mvc;
using tl2_tp8_2025_PauloSrur1.Interfaces;
using tl2_tp8_2025_PauloSrur1.ViewModels;

namespace tl2_tp8_2025_PauloSrur1.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAuthenticationService _authService;
        public LoginController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                model.ErrorMessage = "Debe ingresar usuario y contraseña.";
                return View("Index", model);
            }
            if (_authService.Login(model.Username, model.Password))
            {
                return RedirectToAction("Index", "Home");
            }
            model.ErrorMessage = "Credenciales inválidas.";
            return View("Index", model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            _authService.Logout();
            return RedirectToAction("Index");
        }
    }
}