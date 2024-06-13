using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

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



        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ResponseDto? responseDto = await authService.LoginAsync(model);
            if (responseDto != null && responseDto.IsSuccess)
            {
                
                LoginResponseDto? loginResponse =  JsonConvert.DeserializeObject<LoginResponseDto>
                    (Convert.ToString(responseDto.Result));

                TempData["success"] = "Loging Successful";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("CustomError", responseDto.Message);
                return View(model);
            }
           
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roles = new List<SelectListItem>()
            {
                new SelectListItem{ Text = SD.RoleAdmin, Value = SD.RoleAdmin},
                new SelectListItem{ Text = SD.RoleUser, Value = SD.RoleUser},
            };

            ViewBag.RoleList = roles;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto registerRequestDto)
        {
            var roles = new List<SelectListItem>()
                {
                new SelectListItem{ Text = SD.RoleAdmin, Value = SD.RoleAdmin},
                new SelectListItem{ Text = SD.RoleUser, Value = SD.RoleUser},
                 };
            if (!ModelState.IsValid)
            {
                
                ViewBag.RoleList = roles;
                return View(registerRequestDto);
            }

            ResponseDto? responseDto = await authService.RegisterAsync(registerRequestDto);
            ResponseDto? assignRole;
            if (responseDto != null && responseDto.IsSuccess)
            {
                if (string.IsNullOrEmpty(registerRequestDto.Role))
                {
                    registerRequestDto.Role = SD.RoleUser;
                }
                assignRole = await authService.AssignRoleAsync(registerRequestDto);
                if (assignRole != null && assignRole.IsSuccess)
                {
                    TempData["success"] = "Registration Successful";
                    return RedirectToAction(nameof(Login));
                }
            }

            ModelState.AddModelError("CustomError", responseDto.Message);

            ViewBag.RoleList = roles;
            return View(registerRequestDto);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }

    }
}
