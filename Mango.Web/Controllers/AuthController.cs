using Mango.Web.Models;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequest = new LoginRequestDto();
            return View(loginRequest); 
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

		[HttpGet]
		public IActionResult Logout()
		{
			return View();
		}

	}
}
