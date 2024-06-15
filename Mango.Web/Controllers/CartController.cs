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


		[HttpPost]
		[Authorize]
		public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
		{
			//var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)
			//?.FirstOrDefault()?.Value; //that is how we can get id of logged in user

			//cartDto.CartHeader.UserId = userId;
			var response = await shoppingCartService.ApplyCouponAsync(cartDto);


			if (response != null && response.IsSuccess)
			{
				TempData["success"] = "Cart updated successfully";
				return RedirectToAction(nameof(CartIndex));
			}
			TempData["error"] = "There are no potrivit coupon";
			return RedirectToAction(nameof(CartIndex));
		}
		 
		[HttpPost]
		[Authorize]
		public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
		{
			//var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)
			//?.FirstOrDefault()?.Value; //that is how we can get id of logged in user

			//cartDto.CartHeader.UserId = userId;

			cartDto.CartHeader.CouponCode = "";

			var response = await shoppingCartService.ApplyCouponAsync(cartDto);


			if (response != null && response.IsSuccess)
			{
				TempData["success"] = "Cart updated successfully";
				return RedirectToAction(nameof(CartIndex));
			}
			TempData["error"] = "There are no potrivit coupon";
			return RedirectToAction(nameof(CartIndex));
		}


		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Remove(int cartDetailsId)
		{
			var response = await shoppingCartService.RemoveFromCartAsync(cartDetailsId);

			if (response != null && response.IsSuccess)
			{
				TempData["success"] = "Cart updated successfully";
				return RedirectToAction(nameof(CartIndex));
			}
			return View();
		}

	}
}
