using Mango.Services.ShopingCartApi.Models.DTO;
using Mango.Services.ShopingCartApi.Services.IService;
using Newtonsoft.Json;

namespace Mango.Services.ShopingCartApi.Services
{
	public class CouponService : ICouponService
	{
		private readonly IHttpClientFactory httpClientFactory;

		public CouponService (IHttpClientFactory httpClientFactory)
		{
			this.httpClientFactory = httpClientFactory;
		}
		public async Task<CouponDto> GetCoupon(string couponCode)
		{
			var client = httpClientFactory.CreateClient("Coupon");

			var respons = await client.GetAsync($"/api/coupon/GetByCode/{couponCode}");

			var apiContent = await respons.Content.ReadAsStringAsync();

			var responseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

			if (responseDto.IsSuccess)
			{
				return JsonConvert.DeserializeObject<CouponDto>
					(Convert.ToString(responseDto.Result));
			}

			return new CouponDto();
		}
	}
}
