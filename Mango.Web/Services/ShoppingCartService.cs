using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Services.IService;
using Mango.Web.Utility;

namespace Mango.Web.Services
{
	public class ShoppingCartService : IShoppingCartService
	{
		private readonly IBaseService _baseService;
		public ShoppingCartService(IBaseService baseService)
		{
			_baseService = baseService;
		}

		public async Task<ResponseDto?> ApplyCouponAsync(CartDto cartDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.POST,
				Data = cartDto,
				Url = SD.ShoppingCartApiBase + "/api/cart/ApplyCoupon"
			});
		}

		public async Task<ResponseDto?> EmailCart(CartDto cartDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.POST,
				Data = cartDto,
				Url = SD.ShoppingCartApiBase + "/api/cart/EmailCartRequest"
			});
		}

		public async Task<ResponseDto?> GetCartByUserIdAsnyc(string userId)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.GET,
				Url = SD.ShoppingCartApiBase + "/api/cart/GetCart/" + userId
			});
		}


		public async Task<ResponseDto?> RemoveFromCartAsync(int cartDetailsId)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.POST,
				Data = cartDetailsId,
				Url = SD.ShoppingCartApiBase + "/api/cart/CartRemove"
			});
		}


		public async Task<ResponseDto?> UpsertCartAsync(CartDto cartDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.POST,
				Data = cartDto,
				Url = SD.ShoppingCartApiBase + "/api/cart/CartUpsert"
			});
		}
	}
}
