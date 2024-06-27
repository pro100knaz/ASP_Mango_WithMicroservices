using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Services.IService;
using Mango.Web.Utility;

namespace Mango.Web.Services
{
	public class CouponService : ICouponService
	{
		private readonly IBaseService baseService;

		public CouponService(IBaseService baseService)
		{
			this.baseService = baseService;
		}
		public async Task<ResponseDto?> CreateCouponsAsync(CouponDto couponDto)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.POST,
				Data = couponDto,
				Url = SD.CouponApiBase + "/api/coupon"
			});
		}

		public async Task<ResponseDto?> DeleteCouponseAsync(int id)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.DELETE,
				Url = SD.CouponApiBase + "/api/coupon/" + id
			});
		}

		public async Task<ResponseDto?> GetAllCouponseAsync()
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.GET,
				Url = SD.CouponApiBase + "/api/coupon"
			});
		}

		public async Task<ResponseDto?> GetCouponeAsync(string couponeCode)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.GET,
				Url = SD.CouponApiBase + "/api/coupon/GetByCode/" + couponeCode
			});
		}

		public async Task<ResponseDto?> GetCouponeByIdAsync(int id)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.GET,
				Url = SD.CouponApiBase + "/api/coupon/" + id
			});
		}

		public async Task<ResponseDto?> UpdateCuoponsAsync(CouponDto couponDto)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.PUT,
				Data = couponDto,
				Url = SD.CouponApiBase + "/api/coupon"
			});
		}
	}
}
