using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using static Mango.Web.Utility.SD;
using System;

namespace Mango.Web.Services
{
	public class ProductService : IProductService
	{
		private readonly IBaseService baseService;

		public ProductService(IBaseService baseService)
		{
			this.baseService = baseService;
		}
		public async Task<ResponseDto?> CreateProductAsync(ProductDto productDto)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.POST,
				Data = productDto,
				Url = SD.ProductApiBase + "/api/product"
			});
		}

		public async Task<ResponseDto?> DeleteProductAsync(int id)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.DELETE,

				Url = SD.ProductApiBase + "/api/product/" + id,
			});
		}

		public async Task<ResponseDto?> GetAllProductsAsync()
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.GET,
				Url = SD.ProductApiBase + "/api/product"
			});
		}

		public async Task<ResponseDto?> GetProductByIdAsync(int id)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.GET,

				Url = SD.ProductApiBase + "/api/product/" + id,
			});
		}

		public async Task<ResponseDto?> GetProductByNameAsync(string name)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.GET,

				Url = SD.ProductApiBase + "/api/product/" + name,
			});
		}

		public async Task<ResponseDto?> UpdateProductAsync(ProductDto productDto)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.PUT,
				Data = productDto,
				Url = SD.ProductApiBase + "/api/product"
			});
		}
	}
}
