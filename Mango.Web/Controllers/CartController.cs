using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
	public class CartController : Controller
	{
		private readonly IShoppingCartService shoppingCartService;

		public CartController(IShoppingCartService shoppingCartService)
        {
			this.shoppingCartService = shoppingCartService;
		}
		[Authorize]
        public async Task<IActionResult> CartIndex()
		{
			return View(await LoadCartDtoBasedOnLoggedInUser());
		}

		private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
		{
			var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)
				?.FirstOrDefault()?.Value; //that is how we can get id of logged in user

			if(userId is null)
			{
				TempData["error"] = "Only for Authorized persons";
				RedirectToAction("Index", "Home");
			}

			ResponseDto? response = await shoppingCartService.GetCartByUSerIdAsync(userId);

			if(response != null && response.IsSuccess)
			{
				CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
				return cartDto;
			}

			return new CartDto();
		}

	}
}
