using Mango.Web.Models;
using Mango.Web.Models.DTO;

namespace Mango.Web.Services.IService
{
	public interface ICouponService
	{
		Task<ResponseDto<CouponDto>?> GetCouponeAsync(string couponeCode);
		Task<ResponseDto<List<CouponDto>>?> GetAllCouponseAsync();
		Task<ResponseDto<CouponDto>?> GetCouponeByIdAsync(int id);
		Task<ResponseDto<CouponDto>?> CreateCouponsAsync(CouponDto couponDto);
		Task<ResponseDto<CouponDto>?> UpdateCuoponsAsync(CouponDto couponDto);
		Task<ResponseDto<CouponDto>?> DeleteCouponseAsync(int id);
	}
}
