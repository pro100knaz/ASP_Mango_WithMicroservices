using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Services;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
	public class ProductController : Controller
	{
		private readonly IProductService productService;

		public ProductController(IProductService productService)
        {
			this.productService = productService;
		}     
		public async Task<IActionResult> ProductIndex()
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


		[HttpGet]
		public async Task<IActionResult> ProductCreate()
		{

			return View();
		}


		[HttpPost]
		public async Task<IActionResult> ProductCreate(ProductDto productDto)
		{

			if (ModelState.IsValid)
			{

				ResponseDto? response = await productService.CreateProductAsync(productDto);

				if (response != null && response.IsSuccess)
				{
					TempData["success"] = "Product Created Succesfully";
					return RedirectToAction(nameof(ProductIndex));
				}
				else
				{
					TempData["error"] = response.Message;
				}
			}


			return View(productDto);

		}


		[HttpGet]
		public async Task<IActionResult> ProductDelete(int productId)
		{
			ResponseDto? response = await productService.GetProductByIdAsync(productId);

			if (response != null && response.IsSuccess)
			{
				ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
				return View(model);
			}
			else
			{

				TempData["error"] = response.Message;
			}

			return NotFound();
		}

		[HttpPost]
		public async Task<IActionResult> ProductDelete(ProductDto productDto)
		{
			ResponseDto? response = await productService.DeleteProductAsync(productDto.ProductId);

			if (response != null && response.IsSuccess)
			{
				ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
				TempData["success"] = "Product Deleted Succesfully";
				return RedirectToAction(nameof(ProductIndex));
			}
			else
			{
				TempData["error"] = response.Message;
			}


			return View(productDto);
		}



		[HttpGet]
		public async Task<IActionResult> ProductUpdate(int productId)
		{
			ResponseDto? response = await productService.GetProductByIdAsync(productId);

			if (response != null && response.IsSuccess)
			{
				ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
				return View(model);
			}
			else
			{
				TempData["error"] = response.Message;
			}

			return NotFound();
		}

		[HttpPost]
		public async Task<IActionResult> ProductUpdate(ProductDto productDto)
		{
			ResponseDto? response = await productService.UpdateProductAsync(productDto);

			if (response != null && response.IsSuccess)
			{
				ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
				TempData["success"] = "Product Updated Succesfully";
				return RedirectToAction(nameof(ProductIndex));
			}
			else
			{
				TempData["error"] = response.Message;
			}


			return View(productDto);
		}
	}
}
