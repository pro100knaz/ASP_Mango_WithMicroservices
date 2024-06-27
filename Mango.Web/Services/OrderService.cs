using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Services.IService;
using Mango.Web.Utility;

namespace Mango.Web.Services
{
	public class OrderService : IOrderService
	{
		private readonly IBaseService baseService;

		public OrderService(IBaseService baseService)
		{
			this.baseService = baseService;
		}


		public async Task<ResponseDto?> CreateOrder(CartDto cartDto)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.POST,
				Data = cartDto,
				Url = SD.OrderApiBase + "/api/order/CreateOrder"
			});
		}

		public async Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequestDto)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.POST,
				Data = stripeRequestDto,
				Url = SD.OrderApiBase + "/api/order/CreateStripeSession"
			});
		}

		public async Task<ResponseDto?> Get(string? userId)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.GET,
				//   Data = userId,
				Url = SD.OrderApiBase + "/api/order/GetOrders/" + userId
			});

		}

		public async Task<ResponseDto?> GetOrder(int orderId)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.GET,
				Url = SD.OrderApiBase + "/api/order/GetOrder/" + orderId
			});
		}

		public async Task<ResponseDto?> UpdateOrderStatus(int orderId, string newStatus)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.POST,
				Data = newStatus,
				Url = SD.OrderApiBase + "/api/order/UpdateOrderStatus/" + orderId
			});
		}

		public async Task<ResponseDto?> ValidateStripeSession(int orderHeaderId)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.POST,
				Data = orderHeaderId,
				Url = SD.OrderApiBase + "/api/order/ValidateStripeSession"
			});
		}
	}
}
