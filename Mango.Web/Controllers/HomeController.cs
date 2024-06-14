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

		public HomeController(IProductService productService)
		{
			this.productService = productService;
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
