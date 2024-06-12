using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Services.IService;
using Mango.Web.Utility;

namespace Mango.Web.Services
{
    public class CouponService : ICouponService
    {
		private readonly IBaseService<List<CouponDto>> baseServiceList;
		private readonly IBaseService<CouponDto> baseService;

		public CouponService(IBaseService<CouponDto> baseService, IBaseService<List<CouponDto>> baseServiceList)
        {
			this.baseService = baseService;
			this.baseServiceList = baseServiceList;
		}
        public async Task<ResponseDto<CouponDto>?> CreateCouponsAsync(CouponDto couponDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = couponDto,
                Url = SD.CouponApiBase + "/api/coupon"
            }) ;
		}

        public async Task<ResponseDto<CouponDto>?> DeleteCouponseAsync(int id)
        {
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.DELETE,
				Url = SD.CouponApiBase + "/api/coupon/" + id
			});
		}

        public async Task<ResponseDto<List<CouponDto>>?> GetAllCouponseAsync()
        {
            return await baseServiceList.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponApiBase + "/api/coupon"
            });
        }

        public async Task<ResponseDto<CouponDto>?> GetCouponeAsync(string couponeCode)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponApiBase + "/api/coupon/GetByCode/" + couponeCode
            }) ;
		}

        public async Task<ResponseDto<CouponDto>?> GetCouponeByIdAsync(int id)
        {
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.GET,
				Url = SD.CouponApiBase + "/api/coupon/" + id
			});
		}

        public async Task<ResponseDto<CouponDto>?> UpdateCuoponsAsync(CouponDto couponDto)
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
