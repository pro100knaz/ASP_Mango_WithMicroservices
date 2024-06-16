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
                Url = SD.CouponApiBase + "/api/order/CreateOrder"
            });
        }
    }
}
