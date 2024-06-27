using IdentityModel;
using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
	public class HomeController : Controller
	{

		private readonly IProductService productService;
		private readonly IShoppingCartService shoppingCartService;

		public HomeController(IProductService productService, IShoppingCartService shoppingCartService)
		{
			this.productService = productService;
			this.shoppingCartService = shoppingCartService;
		}



		public async Task<IActionResult> Index()
		{

			List<ProductDto>? list = new();
			ResponseDto? response = await productService.GetAllProductsAsync();

			if (response != null && response.IsSuccess)
			{
				//list = // JsonSerializer.Deserialize<List<ProductDto>>(Convert.ToString(response.Result));
				list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
				// TempData["succes"] = response.Message;
			}
			else
			{
				TempData["error"] = response.Message;
			}

			return View(list);
		}


		[Authorize]
		public async Task<IActionResult> ProductDetails(int productId)
		{

			ProductDto result = new();
			ResponseDto? response = await productService.GetProductByIdAsync(productId);

			if (response != null && response.IsSuccess)
			{
				//list = // JsonSerializer.Deserialize<List<ProductDto>>(Convert.ToString(response.Result));
				result = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
				// TempData["succes"] = response.Message;
			}
			else
			{
				TempData["error"] = response.Message;
			}

			return View(result);
		}


		[Authorize]
		[HttpPost]
		[ActionName("ProductDetails")]
		public async Task<IActionResult> ProductDetails(ProductDto productDto)
		{

			CartDto result = new()
			{
				CartHeader = new CartHeaderDto()
				{
					UserId = User.Claims.Where(u => u.Type == JwtClaimTypes.Subject)?.FirstOrDefault()?.Value,
				}
			};

			CartDetailsDto cartDetails = new()
			{
				Count = productDto.Count,
				ProductId = productDto.ProductId,
			};

			List<CartDetailsDto> cartDetailsDtos = new List<CartDetailsDto>()
			{
				cartDetails
			};

			result.CartDetails = cartDetailsDtos;


			ResponseDto? response = await shoppingCartService.UpsertCartAsync(result);

			if (response != null && response.IsSuccess)
			{
				TempData["success"] = "Item HAs been added to the Shopping Cart";
				return RedirectToAction(nameof(Index));
			}
			else
			{
				TempData["error"] = response.Message;
			}
			return View(result);
		}


		//[HttpPost]
		//[Authorize]
		//public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
		//{
		//	var response = await shoppingCartService.ApplyCouponAsync(cartDto);

		//	if(response != null && response.IsSuccess)
		//	{

		//	}
		//}

		//[HttpPost]
		//[Authorize]
		//public async Task<IActionResult> Remove(int cartDetailsId)
		//{
		//	var response = await shoppingCartService.RemoveFromCartAsync(cartDetailsId);

		//	if (response != null && response.IsSuccess)
		//	{
		//		TempData["success"] = "Cart ipdated successfully";
		//		return RedirectToAction(nameof(Index));
		//	}
		//	return View();
		//}


		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
