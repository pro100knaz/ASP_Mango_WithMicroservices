using Mango.Services.ShopingCartApi.Models.DTO;
using Mango.Services.ShopingCartApi.Services.IService;
using Newtonsoft.Json;

namespace Mango.Services.ShopingCartApi.Services
{
    public class ProductService : IProductService
	{
		private readonly IHttpClientFactory httpClientFactory;

		public ProductService(IHttpClientFactory httpClientFactory)
        {
			this.httpClientFactory = httpClientFactory;
		}
        public async Task<IEnumerable<ProductDto>> GetProducts()
		{


			var client = httpClientFactory.CreateClient("Product");

			var respons = await client.GetAsync($"/api/product");

			var apiContent = await respons.Content.ReadAsStringAsync();	

			var responseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

			if (responseDto.IsSuccess)
			{
				return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>
					(Convert.ToString(responseDto.Result));
			}

			return new List<ProductDto>();

		}
	}
}
