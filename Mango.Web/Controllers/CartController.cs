using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
	public class CartController : Controller
	{
		private readonly IShoppingCartService shoppingCartService;
        private readonly IOrderService orderService;

        public CartController(IShoppingCartService shoppingCartService, IOrderService orderService )
        {
			this.shoppingCartService = shoppingCartService;
            this.orderService = orderService;
        }

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> EmailCart()
		{

			CartDto cart = await LoadCartDtoBasedOnLoggedInUser();

			cart.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;


			ResponseDto? response = await shoppingCartService.EmailCart(cart);
			if (response != null && response.IsSuccess)
			{
				TempData["success"] = "Email will processed and sent shortly";
				return RedirectToAction(nameof(CartIndex));
			}
			TempData["error"] = "There are no potrivit coupon";
			return RedirectToAction(nameof(CartIndex));
		}


		[Authorize]
        public async Task<IActionResult> CartIndex()
		{
			return View(await LoadCartDtoBasedOnLoggedInUser());
		}

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [Authorize]
        public async Task<IActionResult> Confirmation(int orderId)
        {
            var response = await orderService.ValidateStripeSession(orderId);

            if (response != null && response.IsSuccess)
            {

				OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));

				if(orderHeaderDto.Status == SD.Status_Approved)
				{
					return View(orderId);
				}
            }


			//redirect to some error page basd on the status  
            return View(orderId);
        }


        [Authorize]
		[HttpPost]
		[ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            CartDto cart = await LoadCartDtoBasedOnLoggedInUser();

			
			cart.CartHeader.Phone = cartDto.CartHeader.Phone;
			cart.CartHeader.Email = cartDto.CartHeader.Email;
			cart.CartHeader.Name = cartDto.CartHeader.Name;

			var response = await orderService.CreateOrder(cart);

			if( response is not null && response.IsSuccess)
			{
				
				OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));

				//get stripe session and redirect to stripe to place order

				var domain = Request.Scheme + "://" + Request.Host.Value + "/";

				StripeRequestDto stripeRequstDto = new()
				{
					ApprovedUrl = domain + "cart/confirmation?orderId="+ orderHeaderDto.OrderHeaderId,

					CancelUrl = domain + "cart/checkout",

					OrderHeader = orderHeaderDto,

				};

				var stripeResponse = await orderService.CreateStripeSession(stripeRequstDto);


                StripeRequestDto stripeResult = JsonConvert.DeserializeObject<StripeRequestDto>
					(Convert.ToString(stripeResponse.Result));


				Response.Headers.Add("Location", stripeResult.StripeSessionUrl);


				return new StatusCodeResult(303);
            }
            return View(cartDto);

        }


        [Authorize]
		public async Task<IActionResult> ApplyCoupon(CartDto cartDtoInput) //fixed finally
		{

            CartDto cartDto = await LoadCartDtoBasedOnLoggedInUser();
			cartDto.CartHeader.CouponCode = cartDtoInput.CartHeader.CouponCode;

            ResponseDto? response = await shoppingCartService.ApplyCouponAsync(cartDto);


			if (response != null && response.IsSuccess)
			{
				TempData["success"] = "Cart updated successfully";
				return RedirectToAction(nameof(CartIndex));
			}
			TempData["error"] = "There are no potrivit coupon";
			return RedirectToAction(nameof(CartIndex));
		}
		 
		
		[Authorize]
		public async Task<IActionResult> RemoveCoupon()
		{
            CartDto cartDto = await LoadCartDtoBasedOnLoggedInUser();

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

		private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
		{
			var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)
				?.FirstOrDefault()?.Value; //that is how we can get id of logged in user

			if(userId is null)
			{
				TempData["error"] = "Only for Authorized persons";
				RedirectToAction("Index", "Home");
			}

			ResponseDto? response = await shoppingCartService.GetCartByUserIdAsnyc(userId);

			if(response != null && response.IsSuccess)
			{
				CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
				return cartDto;
			}

			return new CartDto();
		}


	}
}
