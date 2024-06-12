using Mango.Web.Models;
using Mango.Web.Models.DTO;

namespace Mango.Web.Services.IService
{
	public interface ICouponService
	{
		Task<ResponseDto?> GetCouponeAsync(string couponeCode);
		Task<ResponseDto?> GetAllCouponseAsync();
		Task<ResponseDto?> GetCouponeByIdAsync(int id);
		Task<ResponseDto?> CreateCouponsAsync(CouponDto couponDto);
		Task<ResponseDto?> UpdateCuoponsAsync(CouponDto couponDto);
		Task<ResponseDto?> DeleteCouponseAsync(int id);
	}
}
