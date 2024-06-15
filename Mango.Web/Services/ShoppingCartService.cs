using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Services.IService;
using Mango.Web.Utility;

namespace Mango.Web.Services
{
	public class ShoppingCartService : IShoppingCartService
	{
		private readonly IBaseService baseService;

		public ShoppingCartService(IBaseService baseService)
		{
			this.baseService = baseService;
		}

		public async Task<ResponseDto?> ApplyCouponAsync(CartDto cartDto)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.POST,
				Data = cartDto,
				Url = SD.ShoppingCartApiBase + "/api/cart/ApplyCoupon"
			});
		}
	

		public async Task<ResponseDto?> GetCartByUSerIdAsync(string userId)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.GET,
				Data = userId,
				Url = SD.ShoppingCartApiBase + "/api/cart/GetCart/" + userId
			});
		}



		public async Task<ResponseDto?> RemoveFromCartAsync(int cartDetailsId)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.POST,
				Data = cartDetailsId,
				Url = SD.ShoppingCartApiBase + "/api/cart/CartRemove/"
			});
		}


		public async Task<ResponseDto?> UpsertCartAsync(CartDto cartDto)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.POST,
				Data = cartDto,
				Url = SD.ShoppingCartApiBase + "/api/cart/CartUpsert"
			});
		}
	}
}
