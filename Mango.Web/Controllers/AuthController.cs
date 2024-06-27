using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
	public class AuthController : Controller
	{
		private readonly IAuthService authService;
		private readonly ITokenProvider tokenProvider;

		public AuthController(IAuthService authService, ITokenProvider tokenProvider)
		{
			this.authService = authService;
			this.tokenProvider = tokenProvider;
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

				LoginResponseDto? loginResponse = JsonConvert.DeserializeObject<LoginResponseDto>
					(Convert.ToString(responseDto.Result));

				await SignInUser(loginResponse);

				tokenProvider.SetToken(loginResponse.Token);
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
		public async Task<IActionResult> Register(RegistrationRequestDto obj)
		{
			ResponseDto result = await authService.RegisterAsync(obj);
			ResponseDto assingRole;

			if (result != null && result.IsSuccess)
			{
				if (string.IsNullOrEmpty(obj.Role))
				{
					obj.Role = SD.RoleUser;
				}
				assingRole = await authService.AssignRoleAsync(obj);
				if (assingRole != null && assingRole.IsSuccess)
				{
					TempData["success"] = "Registration Successful";
					return RedirectToAction(nameof(Login));
				}
			}
			else
			{
				TempData["error"] = result.Message;
			}

			var roleList = new List<SelectListItem>()
			{
				new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin},
				new SelectListItem{Text=SD.RoleUser,Value=SD.RoleUser},
			};

			ViewBag.RoleList = roleList;
			return View(obj);
		}

		[HttpGet]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync();
			tokenProvider.ClearToken();
			return RedirectToAction("Index", "Home");
		}

		private async Task SignInUser(LoginResponseDto model)
		{
			var handler = new JwtSecurityTokenHandler();

			var jwt = handler.ReadJwtToken(model.Token);

			var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

			//var claimList = new List<Claim>
			//{
			//    new Claim(JwtRegisteredClaimNames.Email , applicationUser.Email),
			//    new Claim(JwtRegisteredClaimNames.Sub , applicationUser.Id),
			//    new Claim(JwtRegisteredClaimNames.Name , applicationUser.Name),
			//};
			identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
				jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
			identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
			   jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
			identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
			   jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));



			identity.AddClaim(new Claim(ClaimTypes.Name,
			 jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
			identity.AddClaim(new Claim(ClaimTypes.Role,
				jwt.Claims.FirstOrDefault(u => u.Type == "role").Value)); // to be able to use roles  




			var principle = new ClaimsPrincipal(identity);

			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principle);
		}

	}
}
